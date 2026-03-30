namespace Repository.DataModels.Organization
{
    public class SetupOrganizationDTO
    {
        // Organization fields
        public string OrganizationName { get; set; } = string.Empty;
        public string OrganizationType { get; set; } = string.Empty;
        public string CountryIso3 { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }

        // Manager user fields
        public string ManagerFullName { get; set; } = string.Empty;
        public string ManagerEmail { get; set; } = string.Empty;
        public string ManagerPassword { get; set; } = string.Empty;
        public string? ManagerPhone { get; set; }
        public string? ManagerAddress { get; set; }
    }

    public class OrganizationSetupResultDTO
    {
        public long OrganizationId { get; set; }
        public int UserId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string ManagerEmail { get; set; } = string.Empty;
    }
}
