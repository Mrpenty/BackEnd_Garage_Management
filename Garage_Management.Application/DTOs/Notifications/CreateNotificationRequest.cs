using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.DTOs.Notifications
{
    public class CreateNotificationRequest
    {
        public string Type { get; set; } = string.Empty;           // AppointmentReminder, ...
        public string RecipientType { get; set; } = string.Empty; // SpecificUser, AllCustomers, AllStaff, Group
        public int? SpecificUserId { get; set; }
        public string Channel { get; set; } = string.Empty;       // InApp, Email, SMS, All
        public string Message { get; set; } = string.Empty;       // hỗ trợ placeholder
    }
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
