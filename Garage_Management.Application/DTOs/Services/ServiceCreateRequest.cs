namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceCreateRequest
    {
        public string ServiceName { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
