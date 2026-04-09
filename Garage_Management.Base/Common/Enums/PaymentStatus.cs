using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum PaymentStatus
    {
        Unpaid = 0, //Chưa thanh toán - vừa tạo, chưa trả tiền
        Paid = 1,   //Đã thanh toán - khách đã trả tiền thành công 
        Failed = 2  //Thanh toán thất bại - khách vào trang VNPay nhưng hủy hoặc lỗi
    }
}
