using System;
namespace Domain.Models
{
	public class UserInformation
	{
		    public int UserId { get; set; }
			public required string UserFullName { get; set; }
			public string? Address { get; set; }
            public string? EmailAddress { get; set; }
			public string? MobileNumber { get; set; }


    }
}

