using System;
namespace Domain.Models
{
	public class LoggedInUserInformation
	{
	 public int LoginId { get; set; }
	 public int UserId { get; set; }
	 public string UserName { get; set; }
	 public string UserFullName { get; set; }
	 public string MobileNumber { get; set; }
	 public string EmailAddress { get; set; }

	}
}

