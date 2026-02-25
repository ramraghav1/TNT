using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.AgentLedgerEntry;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/agent-ledger")]
    public class AgentLedgerEntriesController : ControllerBase
    {
        private readonly IAgentLedgerEntryService _service;

        public AgentLedgerEntriesController(IAgentLedgerEntryService service)
        {
            _service = service;
        }

        /// <summary>
        /// Post a manual ledger entry (ADJUSTMENT, DEPOSIT, etc.)
        /// Balance is automatically updated.
        /// </summary>
        [HttpPost]
        public IActionResult PostEntry([FromBody] CreateLedgerEntryRequest request)
        {
            try
            {
                var result = _service.PostEntry(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all ledger entries for an account.
        /// </summary>
        [HttpGet("by-account/{agentAccountId}")]
        public IActionResult GetByAccount(long agentAccountId)
        {
            return Ok(_service.GetByAccount(agentAccountId));
        }

        /// <summary>
        /// Get statement of account with opening/closing balance, totals, and entries.
        /// </summary>
        [HttpGet("statement/{agentAccountId}")]
        public IActionResult GetStatement(long agentAccountId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            try
            {
                var request = new StatementRequest
                {
                    AgentAccountId = agentAccountId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = _service.GetStatement(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
