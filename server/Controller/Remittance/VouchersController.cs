using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.Voucher;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/vouchers")]
    public class VouchersController : ControllerBase
    {
        private readonly IVoucherService _service;

        public VouchersController(IVoucherService service)
        {
            _service = service;
        }

        /// <summary>
        /// Post a voucher entry (DEPOSIT, WITHDRAWAL, ADJUSTMENT).
        /// Balance is automatically updated for the agent or branch.
        /// </summary>
        [HttpPost]
        public IActionResult PostVoucher([FromBody] CreateVoucherRequest request)
        {
            try
            {
                var result = _service.PostVoucher(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all voucher entries.
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        /// <summary>
        /// Get voucher by ID.
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Get vouchers by agent.
        /// </summary>
        [HttpGet("by-agent/{agentId}")]
        public IActionResult GetByAgent(long agentId)
        {
            return Ok(_service.GetByAgent(agentId));
        }

        /// <summary>
        /// Get vouchers by branch.
        /// </summary>
        [HttpGet("by-branch/{branchId}")]
        public IActionResult GetByBranch(long branchId)
        {
            return Ok(_service.GetByBranch(branchId));
        }

        /// <summary>
        /// Get statement of account with opening/closing balance, daily summaries, and all entries.
        /// </summary>
        [HttpGet("statement")]
        public IActionResult GetStatement(
            [FromQuery] string entityType,
            [FromQuery] long? agentId,
            [FromQuery] long? branchId,
            [FromQuery] long? agentAccountId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var request = new AccountStatementRequest
                {
                    EntityType = entityType,
                    AgentId = agentId,
                    BranchId = branchId,
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
