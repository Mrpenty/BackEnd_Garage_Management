namespace Garage_Management.Base.Common.Models.Appointments
{
    public class AppointmentQuery : ParamQuery
    {
        // ví dụ: "pending,confirmed"
        public string? Status { get; set; }
        public int? CustomerId { get; set; }

        /// <summary>
        /// Filter theo chi nhánh. Service tự override theo BranchId của user đang login (non-Admin)
        /// để chống spoof từ FE. Admin có thể nhận null = xem toàn bộ.
        /// </summary>
        public int? BranchId { get; set; }
    }
}
