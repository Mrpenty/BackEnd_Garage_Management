using Garage_Management.Base.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.JobCards
{
    /// <summary>
    /// Bảng WorkBay - Vị trí khoang sửa chữa trong gara
    /// </summary>
    public class WorkBay
    {
        public int Id { get; set; }

        /// <summary>
        /// Phiếu sửa chữa đang được thực hiện tại vị trí này
        /// </summary>
        public int JobcardId { get; set; }
        public JobCard JobCard { get; set; }
        /// <summary>
        /// Tên khu vực / mã khoang (VD: Bay 1..)
        /// </summary>
        public string Name { get; set; }

        public string note { get; set; }
        /// <summary>
        /// Trạng thái hiện tại của khoang
        /// </summary>
        public WorkBayStatus Status { get; set; } = WorkBayStatus.Available;

        /// <summary>
        /// Thời gian tạo khoang sửa chữa
        /// </summary>
        public DateTime CreateAt { get; set; }
        /// <summary>
        /// thời gian cập nhật khoang sửa chữa
        /// </summary>
        public DateTime UpdateAt { get; set; }
        /// <summary>
        /// Thời gian bắt đầu khoang sửa chữa
        /// </summary>
        public DateTime StartAt { get; set; }
        /// <summary>
        /// Thời gian kết thúc khoang sửa chữa
        /// </summary>
        public DateTime EndAt { get; set; }

        /// <summary>
        /// Danh sách lịch sử phiếu sửa chữa đã từng được thực hiện tại vị trí này
        /// </summary>
        public ICollection<JobCard> JobCardHistory { get; set; } = new List<JobCard>();


    }
}
