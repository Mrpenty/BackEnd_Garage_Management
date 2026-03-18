using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Vehiclies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng JobCard - Phiếu sửa chữa / lệnh sửa chữa cho từng xe
    /// </summary>
    public class JobCard : AuditableEntity
    {
       
        public int JobCardId { get; set; }

        /// <summary>
        /// Lịch hẹn gốc được dùng để tạo phiếu
        /// </summary>
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        /// <summary>
        /// Khách hàng sở hữu xe trong phiếu sửa chữa
        /// </summary>
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// Xe được mang đến gara để sửa chữa
        /// </summary>
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        /// <summary>
        /// Ngày bắt đầu tiếp nhận và xử lý phiếu sửa chữa
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Ngày hoàn thành phiếu sửa chữa 
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Trạng thái tổng thể của phiếu sửa chữa
        /// </summary>
        public JobCardStatus Status { get; set; } = JobCardStatus.Created;

        /// <summary>
        /// Phần trăm hoàn thành (0-100)
        /// </summary>
        public int ProgressPercentage { get; set; } = 0;

        /// <summary>
        /// Các bước đã hoàn thành
        /// </summary>
        public string? CompletedSteps { get; set; }

        /// <summary>
        /// Ghi chú tiến độ
        /// </summary>
        public string? ProgressNotes { get; set; }

        /// <summary>
        /// Ghi chú tổng quát cho phiếu (mô tả tình trạng, yêu cầu khách hàng, lưu ý đặc biệt...)
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// supervisor cho phiếu sửa chữa
        /// </summary>
        public int? SupervisorId { get; set; }
        public Employee? Supervisor { get; set; }
        // navigation người tạo
        [ForeignKey("CreatedBy")]
        public Employee? CreatedByEmployee { get; set; }

        // Navigation
        /// <summary>
        /// Danh sách thợ máy được phân công cho phiếu
        /// </summary>
        public ICollection<JobCardMechanic> Mechanics { get; set; } = new List<JobCardMechanic>();

        /// <summary>
        /// Danh sách dịch vụ được thực hiện trong phiếu
        /// </summary>
        public ICollection<JobCardService> Services { get; set; } = new List<JobCardService>();

        /// <summary>
        /// Danh sách phụ tùng được sử dụng trong phiếu
        /// </summary>
        public ICollection<JobCardSparePart> SpareParts { get; set; } = new List<JobCardSparePart>();

        /// <summary>
        /// Các báo giá sửa chữa (repair estimate) liên quan đến phiếu
        /// </summary>
        public ICollection<RepairEstimate> Estimates { get; set; } = new List<RepairEstimate>();

        /// <summary>
        /// Hóa đơn được sinh ra từ phiếu sửa chữa
        /// </summary>
        public Invoice? Invoice { get; set; }

        /// <summary>
        /// Lịch sử thay đổi / nhật ký xử lý phiếu
        /// </summary>
        public ICollection<JobCardLog> Logs { get; set; } = new List<JobCardLog>();
    }
}
