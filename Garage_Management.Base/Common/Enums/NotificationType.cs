using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum NotificationType
    {
        General = 1,      // Thông báo chung (ví dụ: thông báo hệ thống, tin tức)
        JobCardUpdate = 2, // Cập nhật liên quan đến phiếu sửa chữa (ví dụ: trạng thái phiếu, hẹn lấy xe)
        Promotion = 3,     // Thông báo khuyến mãi, ưu đãi
        Reminder = 4       // Nhắc nhở (ví dụ: nhắc lịch hẹn, nhắc bảo dưỡng định kỳ)
    }
}
