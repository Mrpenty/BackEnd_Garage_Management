namespace Garage_Management.Base.Common.Enums
{
    public enum WorkBayStatus
    {
        Available = 1,// Khoang sửa chữa sẵn sàng để sử dụng
        Occupied = 2,// Khoang sửa chữa đang được sử dụng
        Maintenance = 3,// Khoang sửa chữa đang bảo trì
        OutOfService = 4// Khoang sửa chữa không hoạt động
    }
}
