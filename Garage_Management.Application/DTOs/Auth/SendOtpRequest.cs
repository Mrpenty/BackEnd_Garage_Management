using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Auth
{
    public class SendOtpRequest
    {
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại hoặc email")]
        public string PhoneOrEmail { get; set; } = string.Empty;
    }

    public class SendOtpResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty; 
    }
}
