using System;
using Bussiness.Services.Organization;
using Bussiness.Services.Transaction;
using Domain.Models.Organization;
using Domain.Models.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController:ControllerBase
	{
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
		{
            _organizationService = organizationService;
		}
        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganization objCreateOrganization)
        {
            var objReturn = await _organizationService.InsertOrganization(objCreateOrganization);

            if (objReturn > 0)
            {
                return Ok(objReturn);
            }
            else
            {
                return BadRequest();
            }
        }
        [Route("list")]
        [HttpGet]
        public async Task<IActionResult> GetOrganizationList(string fromDate, string toDate)
        {
            var objReturn = await _organizationService.GetDetails();
            return Ok(objReturn);
        }
    }
}

