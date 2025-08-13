using System;
namespace Domain.Models.Organization
{
	public class OrganizationDetail
	{
		
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationType { get; set; }
        public string CountryIso3 { get; set; }
        public string Status { get; set; }
        public string ContactPerson { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
    
	}
}

