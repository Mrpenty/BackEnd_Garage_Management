namespace Garage_Management.Base.Common.Enums
{
    /// <summary>
    /// Trạng thái phân công của thợ máy cho một phiếu sửa chữa
    /// </summary>
    public enum MechanicAssignmentStatus
    {
        /// <summary>Đã phân công nhưng chưa bắt đầu</summary>
        Assigned = 1,

        /// <summary>Đang thực hiện</summary>
        InProgress = 2,

        /// <summary>Đã hoàn thành phần việc được giao</summary>
        Completed = 3,

        /// <summary>Tạm dừng / chờ linh kiện / chờ chỉ đạo</summary>
        OnHold = 4,

        /// <summary>Thợ được rút khỏi phiếu (hủy phân công)</summary>
        Removed = 5
    }
}
