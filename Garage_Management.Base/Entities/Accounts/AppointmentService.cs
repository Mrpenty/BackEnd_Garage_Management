using Garage_Management.Base.Entities.Services;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng trung gian: AppointmentService
    /// Liên kết lịch hẹn với dịch vụ.
    /// </summary>
    public class AppointmentService
    {
        /// <summary>
        /// Id lịch hẹn.
        /// </summary>
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        /// <summary>
        /// Id dịch vụ.
        /// </summary>
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}
