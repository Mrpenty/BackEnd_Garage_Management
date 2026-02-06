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
        /// Danh sách xe cụ thể thuộc model này
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
