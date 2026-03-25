namespace Garage_Management.Application.DTOs.Services
{
    public class ServiceVehicleTypePairResponse
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public bool ServiceIsActive { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; } = string.Empty;
        public bool VehicleTypeIsActive { get; set; }
    }
}
