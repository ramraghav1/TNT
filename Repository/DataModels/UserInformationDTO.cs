using System;
namespace Repository.DataModels
{
	public class UserInformationDTO
	{
		
        public int userid { get; set; }
        public required string userfullname { get; set; }
        public string? address { get; set; }
        public string? emailaddress { get; set; }
        public string? mobilenumber { get; set; }
    
	}
}

