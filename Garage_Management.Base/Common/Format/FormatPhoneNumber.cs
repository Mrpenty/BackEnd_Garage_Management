using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Format
{
    public class FormatPhoneNumber
    {
        public string FormatPhoneNumberHepler(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            // Loại bỏ khoảng trắng, dấu gạch ngang, dấu chấm...
            phoneNumber = new string(phoneNumber.Where(c => char.IsDigit(c) || c == '+').ToArray());

            // Nếu đã có mã quốc gia +84 → giữ nguyên
            if (phoneNumber.StartsWith("+84"))
            {
                if (phoneNumber.Length == 12) // +84 + 9 số
                    return phoneNumber;
            }

            // Nếu bắt đầu bằng 84 (không có +)
            if (phoneNumber.StartsWith("84"))
            {
                if (phoneNumber.Length == 11) // 84 + 9 số
                    return "+" + phoneNumber;
            }

            // Nếu bắt đầu bằng 0 (số Việt Nam phổ biến)
            if (phoneNumber.StartsWith("0"))
            {
                if (phoneNumber.Length == 10 || phoneNumber.Length == 11)
                    return "+84" + phoneNumber.Substring(1);
            }

            // Nếu không có mã quốc gia, giả định là số Việt Nam 9 số (098xxxxxxx)
            if (phoneNumber.Length == 9)
            {
                return "+84" + phoneNumber;
            }

            // Nếu không khớp bất kỳ định dạng nào → trả null để báo lỗi
            return null;
        }
    }
}
