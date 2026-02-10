namespace Garage_Management.Base.Common.Enums
{
    /// <summary>
    /// Trạng thái của dịch vụ trên phiếu sửa chữa
    /// </summary>
    public enum ServiceStatus
    {
        Pending = 1,        // Chưa thực hiện
        InProgress = 2,     // Đang thực hiện
        Completed = 3,      // Hoàn thành
        Cancelled = 4,      // Hủy dịch vụ
        OnHold = 5,         // Tạm dừng (chờ linh kiện, chờ khách xác nhận...)
        UnderWarranty = 6   // Được thực hiện dưới bảo hành (miễn phí)
    }
}