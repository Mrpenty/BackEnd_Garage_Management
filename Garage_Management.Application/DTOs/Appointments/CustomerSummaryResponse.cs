namespace Garage_Management.Application.DTOs.Appointments
{
    public class CustomerSummaryResponse
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? UserId { get; set; }
    }
}
