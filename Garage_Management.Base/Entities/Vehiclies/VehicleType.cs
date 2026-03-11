namespace Garage_Management.Base.Entities.Vehiclies
{
    /// <summary>
    /// Bảng VehicleType - Loại xe (xăng, điện, hybrid, diesel...)
    /// </summary>
    public class VehicleType
    {
        public int VehicleTypeId { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<VehicleModel> VehicleModels { get; set; } = new List<VehicleModel>();
        public ICollection<Garage_Management.Base.Entities.Services.ServiceVehicleType> ServiceVehicleTypes { get; set; } = new List<Garage_Management.Base.Entities.Services.ServiceVehicleType>();
    }
}
