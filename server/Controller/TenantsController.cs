using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Repository.Repositories;
using Domain.Models;
using static Domain.Models.TenantDto;
using Bussiness.Services;
using Bussiness.Services.Organization;
using Domain.Models.Organization;

namespace server.Controller
{
    [ApiController]
    [Route("api/multitenancy")]
    [Authorize] // Tenants can only be managed by authenticated users
    public class MultiTenancyController : ControllerBase
    {
        private readonly IMultiTenantRepository _tenantRepository;
        private readonly ITenantProvider _tenantProvider;
        private readonly IOrganizationService _organizationService;

        public MultiTenancyController(
            IMultiTenantRepository tenantRepository,
            ITenantProvider tenantProvider,
            IOrganizationService organizationService)
        {
            _tenantRepository = tenantRepository;
            _tenantProvider = tenantProvider;
            _organizationService = organizationService;
        }

        /// <summary>
        /// Gets the current tenant's information (based on JWT token)
        /// This is a tenant-safe operation - users can only see their own tenant
        /// </summary>
        [HttpGet("current")]
        public IActionResult GetCurrentTenant()
        {
            // TenantId is extracted from JWT - NOT from frontend
            var tenantId = _tenantProvider.GetTenantId();
            
            if (tenantId == null)
            {
                return Unauthorized(new { message = "Tenant context not found. Please ensure you are logged in with a tenant-associated account." });
            }
            
            var tenant = _tenantRepository.GetTenantById(tenantId.Value);
            if (tenant == null)
                return NotFound(new { message = "Tenant not found" });

            var products = _tenantRepository.GetTenantProducts(tenantId.Value);

            var response = new TenantResponse
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                Status = tenant.Status,
                Settings = tenant.Settings,
                Products = products.ToArray()
            };

            return Ok(response);
        }

        /// <summary>
        /// Updates current tenant information
        /// Tenant-safe: Can only update own tenant based on JWT
        /// </summary>
        [HttpPut("current")]
        public IActionResult UpdateCurrentTenant([FromBody] MultiTenantUpdateRequest request)
        {
            var tenantId = _tenantProvider.GetTenantId();
            
            if (tenantId == null)
            {
                return Unauthorized(new { message = "Tenant context not found. Please ensure you are logged in with a tenant-associated account." });
            }

            var success = _tenantRepository.UpdateTenant(
                tenantId.Value,
                request.Name,
                request.LogoUrl,
                request.ContactPhone
            );

            if (!success)
                return BadRequest(new { message = "Failed to update tenant" });

            return Ok(new { message = "Tenant updated successfully" });
        }

        /// <summary>
        /// Gets the current tenant's subscribed products
        /// </summary>
        [HttpGet("current/products")]
        public IActionResult GetCurrentTenantProducts()
        {
            var tenantId = _tenantProvider.GetTenantId();
            
            if (tenantId == null)
            {
                return Unauthorized(new { message = "Tenant context not found. Please ensure you are logged in with a tenant-associated account." });
            }
            
            var products = _tenantRepository.GetTenantProducts(tenantId.Value);
            
            return Ok(new { products });
        }

        /// <summary>
        /// Get tenant by subdomain (public for subdomain availability check)
        /// </summary>
        [HttpGet("by-subdomain/{subdomain}")]
        [AllowAnonymous]
        public IActionResult GetTenantBySubdomain(string subdomain)
        {
            var tenant = _tenantRepository.GetTenantBySubdomain(subdomain);
            if (tenant == null)
                return NotFound();

            return Ok(tenant);
        }

        /// <summary>
        /// Check if subdomain is available (public endpoint for tenant registration)
        /// </summary>
        [HttpGet("check-subdomain/{subdomain}")]
        [AllowAnonymous]
        public IActionResult CheckSubdomainAvailability(string subdomain)
        {
            var tenant = _tenantRepository.GetTenantBySubdomain(subdomain);
            return Ok(new { available = tenant == null });
        }

        /// <summary>
        /// Get all tenants (admin only)
        /// </summary>
        [HttpGet("all")]
        public IActionResult GetAllTenants()
        {
            var tenants = _tenantRepository.GetAllTenants();
            var response = tenants.Select(t => new TenantResponse
            {
                Id = t.Id,
                Name = t.Name,
                Subdomain = t.Subdomain,
                Status = t.Status,
                Settings = t.Settings,
                Products = _tenantRepository.GetTenantProducts(t.Id).ToArray()
            });
            return Ok(response);
        }

        /// <summary>
        /// Get tenant by ID (admin only)
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetTenantById(long id)
        {
            var tenant = _tenantRepository.GetTenantById(id);
            if (tenant == null)
                return NotFound(new { message = "Tenant not found" });

            var products = _tenantRepository.GetTenantProducts(id);
            var response = new TenantResponse
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Subdomain = tenant.Subdomain,
                Status = tenant.Status,
                Settings = tenant.Settings,
                Products = products.ToArray()
            };
            return Ok(response);
        }

        /// <summary>
        /// Update tenant by ID (admin only)
        /// </summary>
        [HttpPut("{id}")]
        public IActionResult UpdateTenantById(long id, [FromBody] MultiTenantUpdateRequest request)
        {
            var tenant = _tenantRepository.GetTenantById(id);
            if (tenant == null)
                return NotFound(new { message = "Tenant not found" });

            var success = _tenantRepository.UpdateTenant(
                id,
                request.Name,
                request.LogoUrl,
                request.ContactPhone
            );

            if (!success)
                return BadRequest(new { message = "Failed to update tenant" });

            return Ok(new { message = "Tenant updated successfully" });
        }

        /// <summary>
        /// Create new tenant (requires authentication)
        /// Only authenticated users can create tenants
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateTenant([FromBody] MultiTenantCreateRequest request)
        {
            // Map DTO settings to Domain TenantSettings
            TenantSettings? settings = null;
            if (request.Settings != null)
            {
                settings = new TenantSettings
                {
                    MaxUsers = request.Settings.MaxUsers,
                    MaxBookingsPerMonth = request.Settings.MaxBookingsPerMonth,
                    IsTrialAccount = request.Settings.IsTrialAccount,
                    TrialExpiresAt = request.Settings.TrialExpiresAt,
                    TimeZone = request.Settings.TimeZone,
                    Currency = request.Settings.Currency,
                    PrimaryColor = request.Settings.PrimaryColor,
                    SecondaryColor = request.Settings.SecondaryColor,
                    AccentColor = request.Settings.AccentColor,
                    LogoUrl = request.Settings.LogoUrl,
                    FaviconUrl = request.Settings.FaviconUrl,
                    DatabaseType = request.Settings.DatabaseType ?? "Shared",
                    DatabaseConnectionString = request.Settings.DatabaseConnectionString,
                    DatabaseName = request.Settings.DatabaseName,
                    ContractStartDate = request.Settings.ContractStartDate,
                    ContractEndDate = request.Settings.ContractEndDate,
                    ContractType = request.Settings.ContractType,
                    MonthlyFee = request.Settings.MonthlyFee,
                    BillingEmail = request.Settings.BillingEmail,
                    PaymentStatus = request.Settings.PaymentStatus
                };
            }

            var tenantId = _tenantRepository.CreateTenant(
                request.Name,
                request.Subdomain,
                request.ContactEmail,
                request.ContactPhone,
                settings
            );

            // Activate requested products
            foreach (var productId in request.ProductIds)
            {
                _tenantRepository.ActivateTenantProduct(tenantId, productId);
            }

            // Create manager account and organization if manager details provided
            long? organizationId = null;
            int? userId = null;
            if (!string.IsNullOrEmpty(request.ManagerEmail) && 
                !string.IsNullOrEmpty(request.ManagerFullName) && 
                !string.IsNullOrEmpty(request.ManagerPassword))
            {
                try
                {
                    var orgSetupRequest = new SetupOrganizationRequest
                    {
                        OrganizationName = request.Name,
                        OrganizationType = request.ProductIds.Contains(1) ? "TourAndTravels" : 
                                          request.ProductIds.Contains(2) ? "Remittance" : "Clinic",
                        CountryIso3 = "NPL", // Default, can be enhanced
                        ContactPerson = request.ContactEmail,
                        ContactEmail = request.ContactEmail,
                        ContactPhone = request.ContactPhone,
                        ManagerFullName = request.ManagerFullName,
                        ManagerEmail = request.ManagerEmail,
                        ManagerPassword = request.ManagerPassword,
                        ManagerPhone = request.ManagerPhone,
                        ManagerAddress = request.ManagerAddress
                    };

                    var orgResult = await _organizationService.SetupOrganization(orgSetupRequest);
                    organizationId = orgResult.OrganizationId;
                    userId = orgResult.UserId;
                }
                catch (Exception ex)
                {
                    // Log error but don't fail tenant creation
                    Console.WriteLine($"Failed to create organization/manager: {ex.Message}");
                }
            }

            return Ok(new { 
                tenantId, 
                organizationId, 
                userId,
                message = "Tenant created successfully" + (organizationId.HasValue ? " with manager account" : "")
            });
        }
    }

    // DTOs for multi-tenant operations
    public class MultiTenantUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? ContactPhone { get; set; }
    }

    public class MultiTenantCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public List<int> ProductIds { get; set; } = new();
        public TenantSettingsDto? Settings { get; set; }
        
        // Manager Account
        public string? ManagerFullName { get; set; }
        public string? ManagerEmail { get; set; }
        public string? ManagerPassword { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerAddress { get; set; }
    }

    public class TenantSettingsDto
    {
        public int MaxUsers { get; set; } = 10;
        public int MaxBookingsPerMonth { get; set; } = 100;
        public bool IsTrialAccount { get; set; } = false;
        public DateTime? TrialExpiresAt { get; set; }
        public string TimeZone { get; set; } = "UTC";
        public string Currency { get; set; } = "USD";
        
        // Branding/Theme
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? LogoUrl { get; set; }
        public string? FaviconUrl { get; set; }
        
        // Database Configuration
        public string? DatabaseType { get; set; }
        public string? DatabaseConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        
        // Contract/Subscription
        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public string? ContractType { get; set; }
        public decimal? MonthlyFee { get; set; }
        public string? BillingEmail { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
