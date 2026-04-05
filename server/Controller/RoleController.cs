using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services;
using static Repository.DataModels.UserManagementDTO;

namespace server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRolePermissionService _roleService;

        public RoleController(IRolePermissionService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles for a tenant and product
        /// </summary>
        [HttpGet("tenant/{tenantId}/product/{productId}")]
        public IActionResult GetRolesByTenantAndProduct(long tenantId, int productId)
        {
            try
            {
                var roles = _roleService.GetRolesByTenantAndProduct(tenantId, productId);
                return Ok(new { success = true, data = roles });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all available permissions for a product
        /// </summary>
        [HttpGet("permissions/product/{productId}")]
        public IActionResult GetPermissionsByProduct(int productId)
        {
            try
            {
                var permissions = _roleService.GetPermissionsByProduct(productId);
                return Ok(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get user's effective permissions
        /// </summary>
        [HttpGet("permissions/user/{userId}")]
        public IActionResult GetUserPermissions(int userId)
        {
            try
            {
                var permissions = _roleService.GetUserPermissions(userId);
                return Ok(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create a new custom role
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { success = false, message = "Role name is required" });
                }

                var roleId = _roleService.CreateRole(
                    request.TenantId, 
                    request.ProductId, 
                    request.Name, 
                    request.DisplayName ?? request.Name,
                    request.Description
                );
                
                return Ok(new
                {
                    success = true,
                    message = "Role created successfully",
                    data = new { roleId }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing role
        /// </summary>
        [HttpPut("{roleId}")]
        public IActionResult UpdateRole(long roleId, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                _roleService.UpdateRole(
                    roleId, 
                    request.DisplayName, 
                    request.Description, 
                    request.IsActive
                );
                
                return Ok(new { success = true, message = "Role updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a custom role (system roles cannot be deleted)
        /// </summary>
        [HttpDelete("{roleId}")]
        public IActionResult DeleteRole(long roleId)
        {
            try
            {
                _roleService.DeleteRole(roleId);
                return Ok(new { success = true, message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Assign permissions to a role
        /// </summary>
        [HttpPost("{roleId}/permissions")]
        public IActionResult AssignPermissions(long roleId, [FromBody] List<PermissionAssignment> permissions)
        {
            try
            {
                _roleService.AssignPermissionsToRole(roleId, permissions);
                return Ok(new { success = true, message = "Permissions assigned successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions assigned to a role
        /// </summary>
        [HttpGet("{roleId}/permissions")]
        public IActionResult GetRolePermissions(long roleId)
        {
            try
            {
                var permissions = _roleService.GetRolePermissions(roleId);
                return Ok(new { success = true, data = permissions });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }

    // Request DTOs specific to the controller
    public class CreateRoleRequest
    {
        public long TenantId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
