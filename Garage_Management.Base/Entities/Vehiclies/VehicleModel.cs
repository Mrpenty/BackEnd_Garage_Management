using Garage_Management.Base.Entities.Inventories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.Vehiclies
{
    /// <summary>
    /// Bảng VehicleModel - Danh mục dòng xe / model cụ thể
    /// </summary>
    public class VehicleModel
    {
       
        public int ModelId { get; set; }

        /// <summary>
        /// Hãng xe mà model này thuộc về
        /// </summary>
        public int BrandId { get; set; }
      
        public VehicleBrand Brand { get; set; } = null!;

        /// <summary>
        /// Tên model xe (VD: Vios, Civic, CX-5...)
        /// </summary>
      
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Loại xe (xăng, điện, hybrid, diesel...)
        /// </summary>
        public int? VehicleTypeId { get; set; }
        public VehicleType? VehicleType { get; set; }

        /// <summary>
        /// Trạng thái sử dụng
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Danh sách xe cụ thể thuộc model này
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        /// <summary>
        /// Danh sách các phụ tùng phù hợp 
        /// </summary>
        public ICollection<InventoryVehicleModel> CompatibleSpareParts { get; set; } = new List<InventoryVehicleModel>();

    }
}
