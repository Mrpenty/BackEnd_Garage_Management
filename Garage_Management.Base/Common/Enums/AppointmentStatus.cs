namespace Garage_Management.Base.Common.Enums
{
    /// <summary>
    /// Trạng thái của lịch hẹn
    /// </summary>
    public enum AppointmentStatus
    {
        Pending = 1,// Chờ xác nhận / mới đặt
        Confirmed = 2,// Đã xác nhận
        InProgress = 3,// Khách đã đến / đang thực hiện
        NoShow = 4,// Không đến (no-show)
        Cancelled = 5,// Khách hủy
        ConvertedToJobCard = 6, // Đã chuyển thành phiếu sửa chữa
        Completed = 7,// Hoàn thành
    }
}
