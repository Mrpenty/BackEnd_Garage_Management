using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Entities.Accounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng trung gian: JobCardService
    /// Lưu trữ danh sách các dịch vụ được thực hiện trên một phiếu sửa chữa (JobCard)
    /// Bao gồm thông tin chi tiết như giá thực tế, trạng thái, ghi chú...
    /// </summary>
    public class JobCardService
    {
      
        public int JobCardId { get; set; }

       
        public int ServiceId { get; set; }

        // Navigation properties

        /// <summary>
        /// Phiếu sửa chữa mà dịch vụ này được áp dụng
        /// </summary>
    
        public JobCard JobCard { get; set; } = null!;

        /// <summary>
        /// Dịch vụ được thực hiện (từ bảng Services)
        /// </summary>
      
        public Service Service { get; set; } = null!;

       

        /// <summary>
        /// Giá dịch vụ thực tế tại thời điểm thực hiện 
        /// </summary>
        
        public decimal ActualPrice { get; set; }

        /// <summary>
        /// Số lượng (thường là 1 cho dịch vụ, nhưng có thể >1 nếu dịch vụ lặp lại, ví dụ: rửa xe 2 lần)
        /// </summary>
        public int Quantity { get; set; } = 1;

        /// <summary>
        /// Tổng tiền cho dịch vụ này = ActualPrice * Quantity
        /// </summary>
        
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Trạng thái dịch vụ trên phiếu sửa chữa
        /// </summary>
        public ServiceStatus Status { get; set; } = ServiceStatus.Pending;

        /// <summary>
        /// Ghi chú cụ thể cho dịch vụ này (ví dụ: "Thay dầu nhớt 5W-30", "Kiểm tra hệ thống phanh trước")
        /// </summary>
       
        public string? Description { get; set; }

        /// <summary>
        /// Có được bảo hành không? (dựa trên chính sách bảo hành của dịch vụ)
        /// </summary>
        public bool IsUnderWarranty { get; set; } = false;

        /// <summary>
        /// Mã bảo hành (nếu có)
        /// </summary>
        [MaxLength(50)]
        public string? WarrantyCode { get; set; }

        /// <summary>
        /// Người thực hiện / phê duyệt dịch vụ (thường là thợ máy hoặc quản lý)
        /// </summary>
        public int? PerformedBy { get; set; }

      
        public User? PerformedByUser { get; set; }

        /// <summary>
        /// Thời gian bắt đầu thực hiện dịch vụ
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Thời gian hoàn thành dịch vụ
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedBy { get; set; }

        public User CreatedByUser { get; set; } = null!;

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }

        public User? UpdatedByUser { get; set; }
    }
}
