namespace Garage_Management.Base.Common.Enums
{
    /// <summary>
    /// Trạng thái của lịch hẹn
    /// </summary>
    public enum AppointmentStatus
    {
        Pending = 1,// Chờ xác nhận / mới đặt
        Confirmed = 2,// Đã xác nhận
        ConvertedToJobCard = 3, // Đã chuyển thành phiếu sửa chữa
        NoShow = 4,// Không đến (no-show)
        Cancelled = 5,// Khách hủy
        Completed = 6,// Hoàn thành
    }
}
