using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Repository.Repositories;

namespace server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for product management
    public class ProductsController : ControllerBase
    {
        private readonly IMultiTenantRepository _tenantRepository;

        public ProductsController(IMultiTenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        /// <summary>
        /// Get all available products (for tenant product selection)
        /// Requires authentication
        /// </summary>
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            try
            {
                var products = _tenantRepository.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products: {ex.Message}");
                return StatusCode(500, new { message = "Failed to retrieve products", error = ex.Message });
            }
        }
    }
}
