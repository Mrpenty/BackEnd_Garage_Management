namespace Garage_Management.Base.Common.Models.Appointments
{
    public class AppointmentQuery : ParamQuery
    {
        // ví dụ: "pending,confirmed"
        public string? Status { get; set; }
        public int? CustomerId { get; set; }
    }
}
