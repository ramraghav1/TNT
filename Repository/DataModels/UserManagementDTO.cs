namespace Repository.DataModels
{
    public class UserManagementDTO
    {
        public class CreateUserRequest
        {
            public string UserFullName { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;
            public string? MobileNumber { get; set; }
            public string? Address { get; set; }
            public long? TenantId { get; set; }
            public long? OrgId { get; set; }
            public long? BranchId { get; set; }
            public List<long>? RoleIds { get; set; }
            public int CreatedBy { get; set; }
        }

        public class CreateUserResponse
        {
            public int UserId { get; set; }
            public int LoginId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string GeneratedPassword { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;
            public bool EmailSent { get; set; }
        }

        public class UserListDTO
        {
            public int UserId { get; set; }
            public string UserFullName { get; set; } = string.Empty;
            public string EmailAddress { get; set; } = string.Empty;
            public string? MobileNumber { get; set; }
            public long? TenantId { get; set; }
            public string? TenantName { get; set; }
            public long? OrgId { get; set; }
            public string? Roles { get; set; } // Comma-separated role names
            public DateTime CreatedAt { get; set; }
            public bool IsActive { get; set; }
        }

        public class RoleDTO
        {
            public long Id { get; set; }
            public long TenantId { get; set; }
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool IsActive { get; set; }
            public bool IsSystemRole { get; set; }
        }

        public class PermissionDTO
        {
            public long Id { get; set; }
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string DisplayName { get; set; } = string.Empty;
            public string? Category { get; set; }
            public long? ParentId { get; set; }
            public string? Route { get; set; }
            public string? Icon { get; set; }
            public int SortOrder { get; set; }
            public bool CanCreate { get; set; }
            public bool CanRead { get; set; }
            public bool CanUpdate { get; set; }
            public bool CanDelete { get; set; }
        }

        public class RolePermissionDTO
        {
            public long RoleId { get; set; }
            public List<PermissionAssignment> Permissions { get; set; } = new();
        }

        public class PermissionAssignment
        {
            public long PermissionId { get; set; }
            public bool CanCreate { get; set; }
            public bool CanRead { get; set; }
            public bool CanUpdate { get; set; }
            public bool CanDelete { get; set; }
        }

        public class PasswordResetRequest
        {
            public string Email { get; set; } = string.Empty;
        }

        public class PasswordResetResponse
        {
            public string Token { get; set; } = string.Empty;
            public string ResetLink { get; set; } = string.Empty;
            public bool EmailSent { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Token { get; set; } = string.Empty;
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}
