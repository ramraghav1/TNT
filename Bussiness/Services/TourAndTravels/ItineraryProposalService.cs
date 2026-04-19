using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bussiness.Services;
using Domain.Models;
using Domain.Models.TourAndTravels;
using Microsoft.Extensions.Logging;
using Repository.Interfaces.TourAndTravels;
using static Domain.Models.TourAndTravels.ItineraryProposal;

namespace Bussiness.Services.TourAndTravels
{
    public interface IItineraryProposalService
    {
        Task<ProposalResponse> CreateAndSendProposalAsync(CreateProposalRequest request, string frontendBaseUrl, string? adminEmail = null);
        List<ProposalListItem> GetAllProposals();
        ProposalResponse? GetProposalById(long id);
        ProposalResponse? GetProposalByToken(string token);

        // Traveler actions (via token)
        bool AcceptProposal(string token);
        ProposalPaymentResponse SubmitPayment(string token, SubmitProposalPaymentRequest request);
        List<ProposalPaymentResponse> GetPayments(long proposalId);
        ProposalFeedbackResponse SubmitFeedback(string token, SubmitFeedbackRequest request);
        List<ProposalFeedbackResponse> GetFeedback(long proposalId);

        // Admin actions
        bool VerifyPayment(long paymentId, string verifiedBy, VerifyPaymentRequest request);
        Task<ProposalResponse?> ResendProposalAsync(long id, CreateProposalRequest request, string frontendBaseUrl, string? adminEmail = null);
        bool UpdatePaymentScreenshot(long paymentId, string screenshotPath);

        // Traveler reject
        bool RejectProposal(string token);
    }

    public class ItineraryProposalService : IItineraryProposalService
    {
        private readonly IItineraryProposalRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ItineraryProposalService> _logger;

        public ItineraryProposalService(
            IItineraryProposalRepository repository,
            IMapper mapper,
            IEmailService emailService,
            INotificationService notificationService,
            ILogger<ItineraryProposalService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _emailService = emailService;
            _notificationService = notificationService;
            _logger = logger;
        }

        // ===========================
        // Create proposal, send email with link
        // ===========================
        public async Task<ProposalResponse> CreateAndSendProposalAsync(CreateProposalRequest request, string frontendBaseUrl, string? adminEmail = null)
        {
            string token = Guid.NewGuid().ToString("N");

            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryProposalDTO.CreateProposalRequest>(request);
            var repoResponse = _repository.CreateProposal(repoRequest, token);
            var result = _mapper.Map<ProposalResponse>(repoResponse);

            // Send email with the proposal link
            string proposalUrl = $"{frontendBaseUrl}/proposal/{token}";
            string feedbackUrl = $"{frontendBaseUrl}/proposal/{token}/feedback";

            // Send email asynchronously (properly awaited)
            try
            {
                _logger.LogInformation("Attempting to send proposal email to {Email} for itinerary '{Title}'", request.TravelerEmail, result.ItineraryTitle);

                var dayInfos = result.Days.Select(d => new ProposalEmailDayInfo
                {
                    DayNumber = d.DayNumber,
                    Title = d.Title,
                    Description = d.Description,
                    Location = d.Location,
                    Accommodation = d.Accommodation,
                    Transport = d.Transport,
                    BreakfastIncluded = d.BreakfastIncluded,
                    LunchIncluded = d.LunchIncluded,
                    DinnerIncluded = d.DinnerIncluded,
                    Activities = d.Activities,
                    DailyCost = d.DailyCost
                }).ToList();

                var sent = await _emailService.SendItineraryProposalEmailAsync(
                    request.TravelerEmail,
                    request.TravelerName,
                    request.TravelerPhone,
                    result.ItineraryTitle,
                    result.Id,
                    proposalUrl,
                    feedbackUrl,
                    result.TotalAmount,
                    result.Days.Count,
                    result.StartDate,
                    result.EndDate,
                    result.ExpiresAt,
                    request.Notes,
                    dayInfos,
                    adminEmail
                );

                if (!sent)
                    _logger.LogWarning("Email to {Email} returned false (not sent)", request.TravelerEmail);
                else
                    _logger.LogInformation("Proposal email sent to {Email}", request.TravelerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send proposal email to {Email}. Error: {ErrorMessage}", request.TravelerEmail, ex.Message);
            }

            // Notify admin
            try
            {
                await _notificationService.CreateAndBroadcastAsync(new CreateNotification
                {
                    Type = "proposal",
                    Title = "Itinerary Proposal Sent",
                    Message = $"Proposal sent to {request.TravelerName} ({request.TravelerEmail})",
                    Link = "/proposals",
                    Icon = "pi-send"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification for proposal to {Email}", request.TravelerEmail);
            }

            return result;
        }

        // ===========================
        // Get all proposals (admin list)
        // ===========================
        public List<ProposalListItem> GetAllProposals()
        {
            var repoList = _repository.GetAllProposals();
            return _mapper.Map<List<ProposalListItem>>(repoList);
        }

        // ===========================
        // Get proposal by ID (admin)
        // ===========================
        public ProposalResponse? GetProposalById(long id)
        {
            var repo = _repository.GetProposalById(id);
            return repo == null ? null : _mapper.Map<ProposalResponse>(repo);
        }

        // ===========================
        // Get proposal by token (public)
        // ===========================
        public ProposalResponse? GetProposalByToken(string token)
        {
            var repo = _repository.GetProposalByToken(token);
            return repo == null ? null : _mapper.Map<ProposalResponse>(repo);
        }

        // ===========================
        // Traveler accepts proposal
        // ===========================
        public bool AcceptProposal(string token)
        {
            var proposal = _repository.GetProposalByToken(token);
            if (proposal == null) return false;

            bool updated = _repository.UpdateProposalStatus(proposal.Id, "Accepted");

            if (updated)
            {
                _ = _notificationService.CreateAndBroadcastAsync(new CreateNotification
                {
                    Type = "proposal",
                    Title = "Proposal Accepted",
                    Message = $"{proposal.TravelerName} accepted the itinerary proposal for {proposal.ItineraryTitle}",
                    Link = "/proposals",
                    Icon = "pi-check-circle"
                });
            }

            return updated;
        }

        // ===========================
        // Submit payment (traveler via token)
        // ===========================
        public ProposalPaymentResponse SubmitPayment(string token, SubmitProposalPaymentRequest request)
        {
            var proposal = _repository.GetProposalByToken(token)
                ?? throw new Exception("Proposal not found.");

            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryProposalDTO.SubmitProposalPaymentRequest>(request);
            var repoResponse = _repository.SubmitPayment(proposal.Id, repoRequest);
            var result = _mapper.Map<ProposalPaymentResponse>(repoResponse);

            // Notify admin about payment submission
            _ = _notificationService.CreateAndBroadcastAsync(new CreateNotification
            {
                Type = "proposal",
                Title = "Payment Submitted",
                Message = $"{proposal.TravelerName} submitted a {request.PaymentType} payment of {request.Amount:N2} for {proposal.ItineraryTitle}",
                Link = "/proposals",
                Icon = "pi-money-bill"
            });

            return result;
        }

        // ===========================
        // Get payments (admin / public)
        // ===========================
        public List<ProposalPaymentResponse> GetPayments(long proposalId)
        {
            var repoList = _repository.GetPayments(proposalId);
            return _mapper.Map<List<ProposalPaymentResponse>>(repoList);
        }

        // ===========================
        // Submit feedback (traveler via token)
        // ===========================
        public ProposalFeedbackResponse SubmitFeedback(string token, SubmitFeedbackRequest request)
        {
            var proposal = _repository.GetProposalByToken(token)
                ?? throw new Exception("Proposal not found.");

            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryProposalDTO.SubmitFeedbackRequest>(request);
            var repoResponse = _repository.SubmitFeedback(proposal.Id, repoRequest);
            var result = _mapper.Map<ProposalFeedbackResponse>(repoResponse);

            // Notify admin about feedback
            _ = _notificationService.CreateAndBroadcastAsync(new CreateNotification
            {
                Type = "proposal",
                Title = "Traveler Feedback Received",
                Message = $"{proposal.TravelerName} sent feedback: \"{(request.Message.Length > 80 ? request.Message[..80] + "..." : request.Message)}\"",
                Link = "/proposals",
                Icon = "pi-comment"
            });

            return result;
        }

        // ===========================
        // Get feedback (admin)
        // ===========================
        public List<ProposalFeedbackResponse> GetFeedback(long proposalId)
        {
            var repoList = _repository.GetFeedback(proposalId);
            return _mapper.Map<List<ProposalFeedbackResponse>>(repoList);
        }

        // ===========================
        // Verify payment (admin)
        // ===========================
        public bool VerifyPayment(long paymentId, string verifiedBy, VerifyPaymentRequest request)
        {
            return _repository.VerifyPayment(paymentId, verifiedBy, request.Approved, request.Remarks);
        }

        // ===========================
        // Resend proposal (admin re-customizes and re-sends email)
        // ===========================
        public async Task<ProposalResponse?> ResendProposalAsync(long id, CreateProposalRequest request, string frontendBaseUrl, string? adminEmail = null)
        {
            var repoRequest = _mapper.Map<Repository.DataModels.TourAndTravels.ItineraryProposalDTO.CreateProposalRequest>(request);
            var repoResponse = _repository.UpdateProposal(id, repoRequest);
            if (repoResponse == null) return null;

            var result = _mapper.Map<ProposalResponse>(repoResponse);

            // Re-send email
            string proposalUrl = $"{frontendBaseUrl}/proposal/{result.Token}";
            string feedbackUrl = $"{frontendBaseUrl}/proposal/{result.Token}/feedback";

            // Send email asynchronously (properly awaited)
            try
            {
                _logger.LogInformation("Attempting to resend proposal email to {Email} for itinerary '{Title}'", result.TravelerEmail, result.ItineraryTitle);

                var dayInfos = result.Days.Select(d => new ProposalEmailDayInfo
                {
                    DayNumber = d.DayNumber,
                    Title = d.Title,
                    Description = d.Description,
                    Location = d.Location,
                    Accommodation = d.Accommodation,
                    Transport = d.Transport,
                    BreakfastIncluded = d.BreakfastIncluded,
                    LunchIncluded = d.LunchIncluded,
                    DinnerIncluded = d.DinnerIncluded,
                    Activities = d.Activities,
                    DailyCost = d.DailyCost
                }).ToList();

                var sent = await _emailService.SendItineraryProposalEmailAsync(
                    result.TravelerEmail,
                    result.TravelerName,
                    result.TravelerPhone,
                    result.ItineraryTitle,
                    result.Id,
                    proposalUrl,
                    feedbackUrl,
                    result.TotalAmount,
                    result.Days.Count,
                    result.StartDate,
                    result.EndDate,
                    result.ExpiresAt,
                    request.Notes,
                    dayInfos,
                    adminEmail
                );

                if (!sent)
                    _logger.LogWarning("Resend email to {Email} returned false", result.TravelerEmail);
                else
                    _logger.LogInformation("Resend proposal email sent to {Email}", result.TravelerEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend proposal email to {Email}. Error: {ErrorMessage}", result.TravelerEmail, ex.Message);
            }

            // Notify admin
            try
            {
                await _notificationService.CreateAndBroadcastAsync(new CreateNotification
                {
                    Type = "proposal",
                    Title = "Proposal Re-sent",
                    Message = $"Updated proposal re-sent to {result.TravelerName}",
                    Link = "/proposals",
                    Icon = "pi-send"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send resend notification");
            }

            return result;
        }

        // ===========================
        // Update payment screenshot path
        // ===========================
        public bool UpdatePaymentScreenshot(long paymentId, string screenshotPath)
        {
            return _repository.UpdatePaymentScreenshot(paymentId, screenshotPath);
        }

        // ===========================
        // Reject proposal (traveler via token)
        // ===========================
        public bool RejectProposal(string token)
        {
            var proposal = _repository.GetProposalByToken(token);
            if (proposal == null) return false;

            bool updated = _repository.UpdateProposalStatus(proposal.Id, "Rejected");

            if (updated)
            {
                _ = _notificationService.CreateAndBroadcastAsync(new CreateNotification
                {
                    Type = "proposal",
                    Title = "Proposal Rejected",
                    Message = $"{proposal.TravelerName} rejected the itinerary proposal for {proposal.ItineraryTitle}",
                    Link = "/proposals",
                    Icon = "pi-times-circle"
                });
            }

            return updated;
        }
    }
}
