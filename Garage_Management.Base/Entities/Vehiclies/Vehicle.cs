    using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.Vehiclies
{
    /// <summary>
    /// Bảng Vehicle - Thông tin xe của khách hàng
    /// </summary>
    public class Vehicle : AuditableEntity
    {
       
        public int VehicleId { get; set; }

        /// <summary>
        /// Khách hàng sở hữu xe
        /// </summary>
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// Hãng xe (Brand)
        /// </summary>
        public int BrandId { get; set; }
        public VehicleBrand Brand { get; set; } = null!;

        /// <summary>
        /// Dòng xe / model cụ thể
        /// </summary>
        public int ModelId { get; set; }
        public VehicleModel Model { get; set; } = null!;

        /// <summary>
        /// Biển số xe
        /// </summary>
        [MaxLength(20)]
        public string? LicensePlate { get; set; }

        /// <summary>
        /// Năm sản xuất (tùy chọn)
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Số khung (VIN) của xe
        /// </summary>
        [MaxLength(50)]
        public string? Vin { get; set; } // số khung

        /// <summary>
        /// Danh sách phiếu sửa chữa của xe
        /// </summary>
        public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();

        /// <summary>
        /// Danh sách lịch hẹn liên quan đến xe
        /// </summary>
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
