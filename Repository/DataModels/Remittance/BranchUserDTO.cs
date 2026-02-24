using System;

namespace Repository.DataModels.Remittance
{
    public class BranchUserDTO
    {
        public class CreateBranchUserRequest
        {
            public long BranchId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Role { get; set; }
            public string? Username { get; set; }
        }

        public class UpdateBranchUserRequest
        {
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Role { get; set; }
            public string? Username { get; set; }
            public bool? IsActive { get; set; }
        }

        public class BranchUserResponse
        {
            public long Id { get; set; }
            public long BranchId { get; set; }
            public string? BranchName { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Role { get; set; }
            public string? Username { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
