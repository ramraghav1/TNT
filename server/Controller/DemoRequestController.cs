using Bussiness.Services;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoRequestController : ControllerBase
    {
        private readonly IDemoRequestService _demoRequestService;

        public DemoRequestController(IDemoRequestService demoRequestService)
        {
            _demoRequestService = demoRequestService;
        }

        [AllowAnonymous]
        [Route("submit")]
        [HttpPost]
        public async Task<IActionResult> SubmitDemoRequest([FromBody] DemoRequest request)
        {
            try
            {
                var id = await _demoRequestService.SubmitDemoRequest(request);
                if (id > 0) return Ok(new { id, message = "Demo request submitted successfully!" });
                else return BadRequest(new { message = "Failed to submit demo request." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("list")]
        [HttpGet]
        public async Task<IActionResult> GetDemoRequests()
        {
            var result = await _demoRequestService.GetAllDemoRequests();
            return Ok(result);
        }
    }
}
