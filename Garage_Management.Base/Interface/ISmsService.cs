using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Interface
{
    public interface ISmsService
    {
        Task<(bool Success, string Message)> SendOtpAsync(string phoneNumber);
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendNotificationAsync(string phoneNumber, string notification);
        Task<(bool IsValid, string Message)> VerifyOtpAsync(string phoneNumber, string code);
    }
}
