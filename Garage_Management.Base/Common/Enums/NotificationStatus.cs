using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum NotificationStatus
    {
        Pending,// Đang chờ xử lý
        Sent,// Đã gửi thành công
        Delivered,// Đã được người dùng nhận
        Failed// Gửi thất bại
    }
}
