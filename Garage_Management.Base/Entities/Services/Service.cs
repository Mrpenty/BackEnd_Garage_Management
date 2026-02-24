using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.RepairEstimaties;
using Garage_Management.Base.Entities.Warranties;
using System;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Services
{
    /// <summary>
    /// Bảng Service - Danh mục dịch vụ sửa chữa / bảo dưỡng trong gara
    /// </summary>
    public class Service : AuditableEntity
    {
        public int ServiceId { get; set; }

        /// <summary>
        /// Tên dịch vụ (ví dụ: Thay dầu, Bảo dưỡng định kỳ, Kiểm tra phanh,...)
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Giá niêm yết cơ bản của dịch vụ
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Mô tả chi tiết cho dịch vụ
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Cờ đánh dấu dịch vụ còn được sử dụng hay đã ngưng
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Các dòng dịch vụ trên phiếu sửa chữa có sử dụng dịch vụ này
        /// </summary>
        public ICollection<JobCardService> JobCardServices { get; set; } = new List<JobCardService>();
        /// <summary>
        /// Services attached to appointments.
        /// </summary>
        public ICollection<AppointmentService> AppointmentServices { get; set; } = new List<AppointmentService>();
        /// <summary>
        /// Danh sách các công việc chi tiết thuộc dịch vụ này
        /// </summary>
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();
        /// <summary>
        /// Các dòng dịch vụ trong bảng báo giá sửa chữa
        /// </summary>
        public ICollection<RepairEstimateService> RepairEstimateServices { get; set; } = new List<RepairEstimateService>();
        /// <summary>
        /// Các bảo hành dịch vụ liên quan (nếu có chính sách bảo hành riêng)
        /// </summary>
        public ICollection<WarrantyService> WarrantyServices { get; set; } = new List<WarrantyService>();
    }
}
