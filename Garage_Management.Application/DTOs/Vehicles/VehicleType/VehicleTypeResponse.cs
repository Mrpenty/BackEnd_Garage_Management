namespace Garage_Management.Application.DTOs.Vehicles.VehicleType
{
    public class VehicleTypeResponse
    {
        public int VehicleTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}

