using System.ComponentModel.DataAnnotations;

namespace Garage_Management.Application.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
