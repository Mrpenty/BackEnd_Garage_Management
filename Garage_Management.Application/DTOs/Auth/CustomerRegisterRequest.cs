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
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(9, ErrorMessage = "Mật khẩu phải có ít nhất 9 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ hoa, 1 chữ thường và 1 số")]
        public string Password { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Vui lòng nhập xác nhận mật khẩu")]
        //[Compare("Password", ErrorMessage = "MSG08")] 
        //public string ConfirmPassword { get; set; } = string.Empty;

    }
}
