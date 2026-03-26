using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum TransactionType
    {
        Import = 1,            // Nhập hàng từ NCC
        ExportToJobCard = 2,   // Xuất dùng cho JobCard
        ReturnFromJobCard = 3, // Trả ngược từ JobCard về kho
        Adjustment = 4         // Điều chỉnh kiểm kê/thủ công
    }
}
