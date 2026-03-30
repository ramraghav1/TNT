namespace Repository.DataModels
{
    public class DemoRequestDTO
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? CompanyName { get; set; }
        public string ProductInterest { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = "NEW";
        public DateTime CreatedAt { get; set; }
    }
}
