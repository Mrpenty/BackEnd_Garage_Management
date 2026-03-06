using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Enums
{
    public enum WorkingStatus
    {
        Available = 1,     // Rảnh - có thể nhận xe
        Working = 2,       // Đang làm việc / đang sửa xe
        OnBreak = 3,       // Nghỉ giải lao
        OffDuty = 4        // Hết ca / không làm việc
    }
}
