using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Auth
{
    public class CustomerRegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(9, ErrorMessage = "Mật khẩu phải có ít nhất 9 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        public string Password { get; set; } = string.Empty;
    }

    public class VerifyOtpRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Otp { get; set; } = string.Empty;
    }
    public class ResendOtpRequest
    {
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
