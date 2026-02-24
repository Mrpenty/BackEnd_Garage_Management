using Garage_Management.Base.Common.Format;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;

namespace Garage_Management.Base.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _verifyServiceSid;
        private readonly IMemoryCache _cache;

        public TwilioSmsService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"]!;
            _authToken = configuration["Twilio:AuthToken"]!;
            _verifyServiceSid = configuration["Twilio:VerifyServiceSid"];

            TwilioClient.Init(_accountSid, _authToken);
        }

        public async Task<(bool Success, string Message)> SendOtpAsync(string phoneNumber)
        {
            var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(phoneNumber);

            if (string.IsNullOrEmpty(formatted))
            {
                return (false, "Số điện thoại không hợp lệ");
            }
            try
            {
                var verification = await VerificationResource.CreateAsync(
                to: formatted,
                channel: "sms",  // có thể đổi thành "email", "whatsapp"...
                pathServiceSid: _verifyServiceSid
            );

                if (verification.Status == "pending")
                {
                    return (true, "Đã gửi mã OTP thành công");
                }

                return (false, $"Không thể gửi OTP: {verification.Status}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send OTP via Twilio: {ex.Message}", ex);
            }
        }
        // Lưu OTP vào cache khi gửi
        //public async Task SendAndStoreOtpAsync(string phoneNumber, string otp, int userId)
        //{
        //    await SendOtpAsync(phoneNumber, otp);

        //    // Lưu OTP vào cache với thời hạn 5 phút
        //    var cacheKey = $"otp:{userId}";
        //    _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
        //}

        public async Task<(bool IsValid, string Message)> VerifyOtpAsync(string phoneNumber, string code)
        {
            var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(phoneNumber);
            try
            {
                var check = await VerificationCheckResource.CreateAsync(
                    to: formatted,
                    code: code,
                    pathServiceSid: _verifyServiceSid
                );

                if (check.Status == "approved")
                {
                    return (true, "Mã OTP hợp lệ");
                }

                return (false, "Mã OTP không đúng hoặc đã hết hạn");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kiểm tra OTP: {ex.Message}");
            }
        }
        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(phoneNumber);

            try
            {
                var smsMessage = await MessageResource.CreateAsync(
                    from: new PhoneNumber(_verifyServiceSid),
                    to: new PhoneNumber(formatted),
                    body: message
                );

                if (smsMessage.Status == MessageResource.StatusEnum.Failed)
                {
                    throw new Exception($"Failed to send SMS: {smsMessage.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send SMS via Twilio: {ex.Message}", ex);
            }
        }
        public async Task SendNotificationAsync(string phoneNumber, string notification)
        {
            var formatted = new FormatPhoneNumber().FormatPhoneNumberHepler(phoneNumber);
            await SendSmsAsync(formatted, notification);
        }
    }
}