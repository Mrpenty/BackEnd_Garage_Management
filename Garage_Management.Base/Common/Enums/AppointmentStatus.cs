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
        Completed = 4,// Hoàn thành
        Cancelled = 5,// Khách hủy
        NoShow = 6,// Không đến (no-show)
        ConvertedToJobCard = 7 // Đã chuyển thành phiếu sửa chữa
    }
}
