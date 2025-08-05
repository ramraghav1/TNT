using System;
namespace Repository.DataModels
{
	public class LoggedInUserInformationDTO
	{
        
            public int loginid { get; set; }
            public int userid { get; set; }
            public string username { get; set; }
            public string userfullname { get; set; }
            public string mobilenumber { get; set; }
            public string emailaddress { get; set; }

        
    }
}

