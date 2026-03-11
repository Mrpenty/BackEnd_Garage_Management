using Garage_Management.Base.Entities.Vehiclies;

namespace Garage_Management.Base.Entities.Services
{
    /// <summary>
    /// Bảng trung gian ServiceVehicleType - Dịch vụ áp dụng cho loại xe
    /// </summary>
    public class ServiceVehicleType
    {
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;
    }
}
