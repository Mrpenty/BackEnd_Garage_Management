using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Interface
{
    public interface ISmsService
    {
        Task SendOtpAsync(string phoneNumber, string otp);
        Task SendSmsAsync(string phoneNumber, string message);
        Task SendNotificationAsync(string phoneNumber, string notification);
        Task<bool> VerifyOtpAsync(int userId, string otp);
    }
}
