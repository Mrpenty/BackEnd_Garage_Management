using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng Appointment - Lưu thông tin lịch hẹn dịch vụ của khách hàng
    /// </summary>
    public class Appointment : AuditableEntity
    {
      
        public int AppointmentId { get; set; }

     
        public int CustomerId { get; set; }

       
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// Xe của khách hàng 
        /// </summary>
        public int? VehicleId { get; set; }

        /// <summary>
        /// Thông tin xe gắn với lịch hẹn
        /// </summary>
        public Vehicle? Vehicle { get; set; }

        /// <summary>
        /// Nhân viên tiếp nhận / tạo lịch hẹn
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Tài khoản người dùng của nhân viên tạo lịch hẹn
        /// </summary>
        public User? CreatedByUser { get; set; }

        /// <summary>
        /// Thời gian hẹn (ngày + giờ) mà khách hàng đến gara
        /// </summary>
        public DateTime AppointmentDateTime { get; set; }


        /// <summary>
        /// Trạng thái hiện tại của lịch hẹn
        /// </summary>
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        /// <summary>
        /// Ghi chú từ khách hàng hoặc nhân viên (mô tả vấn đề xe, yêu cầu đặc biệt...)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Tham chiếu đến phiếu sửa chữa nếu lịch hẹn đã được chuyển đổi
        /// </summary>
        public int? JobCardId { get; set; }

        /// <summary>
        /// Phiếu sửa chữa được tạo ra từ lịch hẹn (nếu có)
        /// </summary>
        public JobCard? JobCard { get; set; }

        /// <summary>
        /// Tài khoản người dùng tương ứng với UpdatedBy
        /// </summary>
        public User? UpdatedByUser { get; set; }

        /// <summary>
        /// Các phiếu sửa chữa được sinh ra từ lịch hẹn (trong trường hợp 1 lịch có thể tách thành nhiều phiếu)
        /// </summary>
        public ICollection<JobCard> GeneratedJobCards { get; set; } = new List<JobCard>();
    }
}
