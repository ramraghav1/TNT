using System;
namespace Repository.DataModels.Organization
{
	public class OrganizationDetailDTO
	{
        public long organization_id { get; set; }
        public string organization_name { get; set; }
        public string organization_type { get; set; }
        public string country_iso3 { get; set; }
        public string status { get; set; }
        public string contact_person { get; set; }
        public string contact_email { get; set; }
        public string contact_phone { get; set; }
    }
}

