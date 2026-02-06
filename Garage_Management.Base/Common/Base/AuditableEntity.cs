using System;

namespace Garage_Management.Base.Common.Base
{
    /// <summary>
    /// Base class dùng chung cho các bảng có thông tin audit (tạo / cập nhật / xóa mềm).
    /// </summary>
    public abstract class AuditableEntity
    {
        /// <summary>
        /// Thời điểm tạo bản ghi.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Người tạo bản ghi (UserId).
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Thời điểm cập nhật gần nhất.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Người cập nhật gần nhất 
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// Thời điểm xóa mềm 
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// Người thực hiện xóa mềm 
        /// </summary>
        public int? DeletedBy { get; set; }
    }
}


