using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Base.Entities.Vehiclies
{
    /// <summary>
    /// Bảng VehicleBrand - Danh mục hãng xe (Toyota, Honda, BMW, ...)
    /// </summary>
    public class VehicleBrand
    {
      
        public int BrandId { get; set; }

        /// <summary>
        /// Tên hãng xe
        /// </summary>
        
        public string BrandName { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách các dòng xe thuộc hãng
        /// </summary>
        public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();

        /// <summary>
        /// Danh sách xe cụ thể thuộc hãng (tiện cho truy vấn nhanh)
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
