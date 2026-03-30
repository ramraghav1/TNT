namespace Domain.Models
{
    public class DemoRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string ProductInterest { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
