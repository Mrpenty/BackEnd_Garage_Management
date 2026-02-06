namespace Garage_Management.Base.Common.Enums
{
    /// <summary>
    /// Trạng thái của lịch hẹn
    /// </summary>
    public enum AppointmentStatus
    {
        /// <summary>Chờ xác nhận / mới đặt</summary>
        Pending = 1,

        /// <summary>Đã xác nhận</summary>
        Confirmed = 2,

        /// <summary>Khách đã đến / đang thực hiện</summary>
        InProgress = 3,

        /// <summary>Hoàn thành</summary>
        Completed = 4,

        /// <summary>Khách hủy</summary>
        Cancelled = 5,

        /// <summary>Không đến (no-show)</summary>
        NoShow = 6,

        /// <summary>Đã chuyển thành phiếu sửa chữa</summary>
        ConvertedToJobCard = 7
    }
}
