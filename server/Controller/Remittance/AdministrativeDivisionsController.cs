using Microsoft.AspNetCore.Mvc;
using Bussiness.Services.Remittance;

namespace server.Controller.Remittance
{
    [ApiController]
    [Route("api/remittance/administrative-divisions")]
    public class AdministrativeDivisionsController : ControllerBase
    {
        private readonly IAdministrativeDivisionService _service;

        public AdministrativeDivisionsController(IAdministrativeDivisionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all divisions for a country (all levels, flat list)
        /// </summary>
        [HttpGet("by-country/{countryId}")]
        public IActionResult GetByCountry(long countryId)
        {
            return Ok(_service.GetByCountry(countryId));
        }

        /// <summary>
        /// Get divisions by country and level (e.g. level=1 for provinces)
        /// </summary>
        [HttpGet("by-country/{countryId}/level/{level}")]
        public IActionResult GetByCountryAndLevel(long countryId, int level)
        {
            return Ok(_service.GetByCountryAndLevel(countryId, level));
        }

        /// <summary>
        /// Get children of a given parent division (cascading)
        /// </summary>
        [HttpGet("children/{parentId}")]
        public IActionResult GetChildren(long parentId)
        {
            return Ok(_service.GetChildren(parentId));
        }
    }
}
