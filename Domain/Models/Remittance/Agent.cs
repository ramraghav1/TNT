using System;

namespace Domain.Models.Remittance
{
    public class Agent
    {
        // ===========================
        // Create / Update
        // ===========================
        public class CreateAgentRequest
        {
            public string Name { get; set; } = string.Empty;
            public long CountryId { get; set; }
            public long? CategoryId { get; set; }
            public string? AgentType { get; set; }
            public string? Address { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
        }

        public class UpdateAgentRequest
        {
            public string? Name { get; set; }
            public long? CategoryId { get; set; }
            public string? AgentType { get; set; }
            public string? Address { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public bool? IsActive { get; set; }
        }

        // ===========================
        // Response
        // ===========================
        public class AgentResponse
        {
            public long Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public long CountryId { get; set; }
            public string? CountryName { get; set; }
            public long? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public string? AgentType { get; set; }
            public string? Address { get; set; }
            public string? ContactPerson { get; set; }
            public string? ContactEmail { get; set; }
            public string? ContactPhone { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
