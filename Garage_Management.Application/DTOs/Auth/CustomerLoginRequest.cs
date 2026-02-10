using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Auth
{
    public class CustomerLoginRequest
    {
        
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Mật khẩu (dùng khi đăng nhập bằng password)
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Mã OTP (dùng khi đăng nhập bằng OTP)
        /// </summary>
        public string? Otp { get; set; }

        /// <summary>
        /// Chọn phương thức đăng nhập: true = OTP, false = Password
        /// </summary>
        public bool UseOtp { get; set; } = false;
    }
}
