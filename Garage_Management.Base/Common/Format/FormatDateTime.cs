using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Format
{
    public class FormatDateTime
    {
        /// <summary>
        /// Chuyển đổi chuỗi ngày giờ bất kỳ thành định dạng "dd-MM-yyyy".
        /// Hỗ trợ các định dạng phổ biến: ISO 8601, dd/MM/yyyy, yyyy-MM-dd, dd-MM-yyyy...
        /// </summary>
        /// <param name="dateString">Chuỗi ngày giờ cần format (có thể null hoặc rỗng)</param>
        /// <returns>Chuỗi "dd-MM-yyyy" nếu hợp lệ, ngược lại trả về null hoặc chuỗi lỗi</returns>
        public string FormatToDdMmYyyy(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;
            string[] formats = new[]
            {
            "yyyy-MM-ddTHH:mm:ss.fffffff",   // ISO 8601 đầy đủ (như "2026-02-23T03:01:52.9769524")
            "yyyy-MM-ddTHH:mm:ss.fff",       // ISO rút gọn
            "yyyy-MM-dd",                    
            "dd/MM/yyyy",                    
            "dd-MM-yyyy",                    
            "MM/dd/yyyy",                     
            "yyyy/MM/dd"                     
        };

            if (DateTime.TryParseExact(dateString.Trim(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("dd-MM-yyyy");
            }
            // Nếu không parse được → thử parse thông thường (DateTime.Parse)
            if (DateTime.TryParse(dateString.Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate.ToString("dd-MM-yyyy");
            }
            return null;
        }

        /// <summary>
        /// Phiên bản trả về chuỗi rỗng nếu không parse được (thay vì null)
        /// </summary>
        public string FormatToDdMmYyyySafe(string dateString)
        {
            return FormatToDdMmYyyy(dateString) ?? "";
        }
    }
}
