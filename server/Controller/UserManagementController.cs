using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Services;
using static Repository.DataModels.UserManagementDTO;

namespace server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly IRolePermissionService _roleService;

        public UserManagementController(
            IUserManagementService userService,
            IRolePermissionService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        /// <summary>
        /// Create a new user with auto-generated password and email notification
        /// </summary>
        [HttpPost("create")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var result = _userService.CreateUser(request, baseUrl);
                
                return Ok(new
                {
                    success = true,
                    message = "User created successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Create multiple users at once (for the accordion form feature)
        /// </summary>
        [HttpPost("create-bulk")]
        public IActionResult CreateBulkUsers([FromBody] List<CreateUserRequest> requests)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var results = new List<CreateUserResponse>();
                var errors = new List<string>();

                foreach (var request in requests)
                {
                    try
                    {
                        var result = _userService.CreateUser(request, baseUrl);
                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Failed to create {request.EmailAddress}: {ex.Message}");
                    }
                }

                return Ok(new
                {
                    success = true,
                    message = $"Created {results.Count} out of {requests.Count} users",
                    data = results,
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all users by tenant
        /// </summary>
        [HttpGet("tenant/{tenantId}")]
        public IActionResult GetUsersByTenant(long tenantId)
        {
            try
            {
                var users = _userService.GetUsersByTenant(tenantId);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Get all users by organization
        /// </summary>
        [HttpGet("organization/{orgId}")]
        public IActionResult GetUsersByOrganization(long orgId)
        {
            try
            {
                var users = _userService.GetUsersByOrganization(orgId);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a user (removes login access)
        /// </summary>
        [HttpPost("{userId}/deactivate")]
        public IActionResult DeactivateUser(int userId)
        {
            try
            {
                _userService.DeactivateUser(userId);
                return Ok(new { success = true, message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reactivate a user
        /// </summary>
        [HttpPost("{userId}/reactivate")]
        public IActionResult ReactivateUser(int userId)
        {
            try
            {
                _userService.ReactivateUser(userId);
                return Ok(new { success = true, message = "User reactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Request password reset
        /// </summary>
        [AllowAnonymous]
        [HttpPost("password-reset/request")]
        public IActionResult RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var result = _userService.RequestPasswordReset(request.Email, baseUrl);
                
                return Ok(new
                {
                    success = true,
                    message = "Password reset email sent",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Reset password with token
        /// </summary>
        [AllowAnonymous]
        [HttpPost("password-reset/confirm")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                bool success = _userService.ResetPassword(request.Token, request.NewPassword);
                
                if (success)
                {
                    return Ok(new { success = true, message = "Password reset successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Invalid or expired token" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
