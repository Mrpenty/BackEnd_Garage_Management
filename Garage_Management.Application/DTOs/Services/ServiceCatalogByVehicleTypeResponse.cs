namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceCatalogByVehicleTypeResponse
    {
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<ServiceResponse> Services { get; set; } = new();
    }
}
