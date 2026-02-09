using Garage_Management.Base.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Garage_Management.Base.Service
{
    public class TwilioSmsService : ISmsService
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioPhoneNumber;
        private readonly IMemoryCache _cache;

        public TwilioSmsService(IConfiguration configuration)
        {
            _accountSid = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            _twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
        }

        public async Task SendOtpAsync(string phoneNumber, string otp)
        {
            try
            {
                var message = await MessageResource.CreateAsync(
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber),
                    body: $"Mã OTP của bạn là: {otp}. Mã có hiệu lực trong 5 phút."
                );

                if (message.Status == MessageResource.StatusEnum.Failed)
                {
                    throw new Exception($"Failed to send SMS: {message.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send OTP via Twilio: {ex.Message}", ex);
            }
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var smsMessage = await MessageResource.CreateAsync(
                    from: new PhoneNumber(_twilioPhoneNumber),
                    to: new PhoneNumber(phoneNumber),
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
        public async Task<bool> VerifyOtpAsync(int userId, string otp)
        {
            var cacheKey = $"otp:{userId}";
            if (!_cache.TryGetValue(cacheKey, out string storedOtp) || storedOtp != otp)
            {
                return false;
            }

            // Xóa OTP sau khi verify thành công
            _cache.Remove(cacheKey);
            return true;
        }
        public async Task SendNotificationAsync(string phoneNumber, string notification)
        {
            await SendSmsAsync(phoneNumber, notification);
        }

        public async Task SendZaloMessageAsync(string phoneNumber, string message)
        {
            await SendSmsAsync(phoneNumber, message);
        }
    }
}