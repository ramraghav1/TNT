using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bussiness.Services;
using Bussiness.Services.TourAndTravels;
using static Domain.Models.TourAndTravels.ItineraryProposal;

namespace server.Controller.TourAndTravels
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProposalsController : ControllerBase
    {
        private readonly IItineraryProposalService _service;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;

        public ProposalsController(IItineraryProposalService service, IWebHostEnvironment env, IEmailService emailService)
        {
            _service = service;
            _env = env;
            _emailService = emailService;
        }

        // ===========================
        // TEST EMAIL (temporary debug endpoint)
        // GET /api/proposals/test-email?to=someone@email.com
        // ===========================
        [HttpGet("test-email")]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmail([FromQuery] string to)
        {
            if (string.IsNullOrWhiteSpace(to))
                return BadRequest(new { message = "Provide ?to=email@example.com" });

            var (success, message) = await _emailService.SendTestEmailAsync(to);
            return Ok(new { success, message, sentTo = to, sentAt = DateTime.Now });
        }

        // ===========================
        // Create and send proposal email (admin)
        // POST /api/proposals/create
        // ===========================
        [HttpPost("create")]
        [Authorize]
        public async Task<ActionResult<ProposalResponse>> CreateProposal([FromBody] CreateProposalRequest request)
        {
            try
            {
                string frontendBaseUrl = Request.Headers["X-Frontend-Url"].ToString();
                if (string.IsNullOrEmpty(frontendBaseUrl))
                    frontendBaseUrl = "http://localhost:4200";

                // Get current logged-in user's email for CC
                string? adminEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;

                var result = await _service.CreateAndSendProposalAsync(request, frontendBaseUrl, adminEmail);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] CreateProposal failed: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { message = "Failed to create proposal.", detail = ex.Message });
            }
        }

        // ===========================
        // Get all proposals (admin)
        // GET /api/proposals
        // ===========================
        [HttpGet]
        [Authorize]
        public ActionResult<List<ProposalListItem>> GetAll()
        {
            return Ok(_service.GetAllProposals());
        }

        // ===========================
        // Get proposal by ID (admin)
        // GET /api/proposals/{id}
        // ===========================
        [HttpGet("{id:long}")]
        [Authorize]
        public ActionResult<ProposalResponse> GetById(long id)
        {
            var proposal = _service.GetProposalById(id);
            if (proposal == null) return NotFound();
            return Ok(proposal);
        }

        // ===========================
        // Get proposal by token (public - no auth)
        // GET /api/proposals/public/{token}
        // ===========================
        [HttpGet("public/{token}")]
        [AllowAnonymous]
        public ActionResult<ProposalResponse> GetByToken(string token)
        {
            var proposal = _service.GetProposalByToken(token);
            if (proposal == null) return NotFound(new { message = "Proposal not found or expired." });
            return Ok(proposal);
        }

        // ===========================
        // Accept proposal (traveler - public)
        // POST /api/proposals/public/{token}/accept
        // ===========================
        [HttpPost("public/{token}/accept")]
        [AllowAnonymous]
        public IActionResult AcceptProposal(string token)
        {
            var result = _service.AcceptProposal(token);
            if (!result) return NotFound(new { message = "Proposal not found." });
            return Ok(new { message = "Proposal accepted successfully." });
        }

        // ===========================
        // Submit payment (traveler - public)
        // POST /api/proposals/public/{token}/payment
        // ===========================
        [HttpPost("public/{token}/payment")]
        [AllowAnonymous]
        public ActionResult<ProposalPaymentResponse> SubmitPayment(string token, [FromBody] SubmitProposalPaymentRequest request)
        {
            try
            {
                var result = _service.SubmitPayment(token, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // Upload payment screenshot (traveler - public)
        // POST /api/proposals/public/payment/{paymentId}/screenshot
        // ===========================
        [HttpPost("public/payment/{paymentId:long}/screenshot")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadScreenshot(long paymentId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { message = "Only image files (jpg, png, webp) are allowed." });

            // Max 5 MB
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size must be less than 5MB." });

            // Save file
            var uploadsDir = Path.Combine(_env.ContentRootPath, "uploads", "payment-screenshots");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{paymentId}_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/payment-screenshots/{fileName}";
            _service.UpdatePaymentScreenshot(paymentId, relativePath);

            return Ok(new { path = relativePath, message = "Screenshot uploaded successfully." });
        }

        // ===========================
        // Get payments for proposal (admin)
        // GET /api/proposals/{id}/payments
        // ===========================
        [HttpGet("{id:long}/payments")]
        [Authorize]
        public ActionResult<List<ProposalPaymentResponse>> GetPayments(long id)
        {
            return Ok(_service.GetPayments(id));
        }

        // ===========================
        // Verify payment (admin)
        // POST /api/proposals/payment/{paymentId}/verify
        // ===========================
        [HttpPost("payment/{paymentId:long}/verify")]
        [Authorize]
        public IActionResult VerifyPayment(long paymentId, [FromBody] VerifyPaymentRequest request)
        {
            // Get verifiedBy from JWT claims
            var verifiedBy = User.FindFirst("name")?.Value ?? User.FindFirst("sub")?.Value ?? "admin";
            var result = _service.VerifyPayment(paymentId, verifiedBy, request);
            if (!result) return NotFound(new { message = "Payment not found." });
            return Ok(new { message = request.Approved ? "Payment verified." : "Payment rejected." });
        }

        // ===========================
        // Submit feedback (traveler - public)
        // POST /api/proposals/public/{token}/feedback
        // ===========================
        [HttpPost("public/{token}/feedback")]
        [AllowAnonymous]
        public ActionResult<ProposalFeedbackResponse> SubmitFeedback(string token, [FromBody] SubmitFeedbackRequest request)
        {
            try
            {
                var result = _service.SubmitFeedback(token, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ===========================
        // Get feedback for proposal (admin)
        // GET /api/proposals/{id}/feedback
        // ===========================
        [HttpGet("{id:long}/feedback")]
        [Authorize]
        public ActionResult<List<ProposalFeedbackResponse>> GetFeedback(long id)
        {
            return Ok(_service.GetFeedback(id));
        }

        // ===========================
        // Resend proposal (admin re-customizes and re-sends)
        // PUT /api/proposals/{id}/resend
        // ===========================
        [HttpPut("{id:long}/resend")]
        [Authorize]
        public async Task<ActionResult<ProposalResponse>> ResendProposal(long id, [FromBody] CreateProposalRequest request)
        {
            string frontendBaseUrl = Request.Headers["X-Frontend-Url"].ToString();
            if (string.IsNullOrEmpty(frontendBaseUrl))
                frontendBaseUrl = "http://localhost:4200";

            string? adminEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                ?? User.FindFirst("email")?.Value;

            var result = await _service.ResendProposalAsync(id, request, frontendBaseUrl, adminEmail);
            if (result == null) return NotFound(new { message = "Proposal not found." });
            return Ok(result);
        }

        // ===========================
        // Get payments by token (public - for traveler view)
        // GET /api/proposals/public/{token}/payments
        // ===========================
        [HttpGet("public/{token}/payments")]
        [AllowAnonymous]
        public ActionResult<List<ProposalPaymentResponse>> GetPaymentsByToken(string token)
        {
            var proposal = _service.GetProposalByToken(token);
            if (proposal == null) return NotFound();
            return Ok(_service.GetPayments(proposal.Id));
        }

        // ===========================
        // Reject proposal (traveler - public)
        // POST /api/proposals/public/{token}/reject
        // ===========================
        [HttpPost("public/{token}/reject")]
        [AllowAnonymous]
        public IActionResult RejectProposal(string token)
        {
            var result = _service.RejectProposal(token);
            if (!result) return NotFound(new { message = "Proposal not found." });
            return Ok(new { message = "Proposal rejected." });
        }

        // ===========================
        // Get invoice data (public - for confirmed proposals)
        // GET /api/proposals/public/{token}/invoice
        // ===========================
        [HttpGet("public/{token}/invoice")]
        [AllowAnonymous]
        public ActionResult GetInvoice(string token)
        {
            var proposal = _service.GetProposalByToken(token);
            if (proposal == null) return NotFound(new { message = "Proposal not found." });
            if (proposal.Status != "Confirmed")
                return BadRequest(new { message = "Invoice only available for confirmed bookings." });

            var payments = _service.GetPayments(proposal.Id);
            return Ok(new
            {
                proposal,
                payments,
                invoiceNumber = $"INV-{DateTime.Now:yyyy}-{proposal.Id:D4}",
                generatedAt = DateTime.Now
            });
        }
    }
}
