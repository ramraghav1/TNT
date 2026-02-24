using System;

namespace Repository.DataModels.Remittance
{
    public class BranchDTO
    {
        public class CreateBranchRequest
        {
            public long AgentId { get; set; }
            public string BranchName { get; set; } = string.Empty;
            public string? BranchCode { get; set; }
            public string? Address { get; set; }
            public string? State { get; set; }
            public string? District { get; set; }
            public string? Locallevel { get; set; }
            public int? WardNumber { get; set; }
            public string? Zipcode { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
        }

        public class UpdateBranchRequest
        {
            public string? BranchName { get; set; }
            public string? BranchCode { get; set; }
            public string? Address { get; set; }
            public string? State { get; set; }
            public string? District { get; set; }
            public string? Locallevel { get; set; }
            public int? WardNumber { get; set; }
            public string? Zipcode { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public bool? IsActive { get; set; }
        }

        public class BranchResponse
        {
            public long Id { get; set; }
            public long AgentId { get; set; }
            public string? AgentName { get; set; }
            public string BranchName { get; set; } = string.Empty;
            public string? BranchCode { get; set; }
            public string? Address { get; set; }
            public string? State { get; set; }
            public string? District { get; set; }
            public string? Locallevel { get; set; }
            public int? WardNumber { get; set; }
            public string? Zipcode { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
