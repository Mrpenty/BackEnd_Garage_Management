using Garage_Management.Base.Entities.Inventories;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng trung gian: AppointmentSparePart
    /// Liên kết lịch hẹn với phụ tùng (Inventory).
    /// </summary>
    public class AppointmentSparePart
    {
        /// <summary>
        /// Id lịch hẹn.
        /// </summary>
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        /// <summary>
        /// Id phụ tùng.
        /// </summary>
        public int SparePartId { get; set; }
        public Inventory Inventory { get; set; } = null!;
    }
}
