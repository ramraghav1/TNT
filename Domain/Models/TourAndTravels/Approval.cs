using System;
namespace Domain.Models.TourAndTravels
{
	public class Approval
	{
        public class ApproveRequest
        {
            public bool Approved { get; set; }
            public string? Remarks { get; set; }
        }
    }
}

