using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;
using static Domain.Models.Remittance.BranchUser;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/branch-users")]
    public class BranchUsersController : ControllerBase
    {
        private readonly IBranchUserService _service;

        public BranchUsersController(IBranchUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateBranchUserRequest request)
        {
            var result = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("by-branch/{branchId}")]
        public IActionResult GetByBranch(long branchId)
        {
            return Ok(_service.GetByBranch(branchId));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            var result = _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] UpdateBranchUserRequest request)
        {
            var result = _service.Update(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            return _service.Delete(id) ? NoContent() : NotFound();
        }
    }
}
