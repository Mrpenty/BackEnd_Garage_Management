namespace Garage_Management.Application.DTOs.Reports
{
    public class ReceptionistReportResponse
    {
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int TotalAppointments { get; set; }
        public AppointmentStatusBreakdown AppointmentsByStatus { get; set; } = new();

        /// <summary>Tỷ lệ khách hẹn nhưng không đến (NoShow / Total).</summary>
        public double NoShowRate { get; set; }

        /// <summary>Tỷ lệ huỷ lịch (Cancelled / Total).</summary>
        public double CancelRate { get; set; }

        /// <summary>Tỷ lệ chuyển từ lịch hẹn → phiếu sửa chữa (ConvertedToJobCard + Completed) / Total.</summary>
        public double ConversionRate { get; set; }

        /// <summary>Số lượt khách walk-in (phiếu sửa không có AppointmentId).</summary>
        public int WalkInCount { get; set; }

        /// <summary>Số phiếu sửa được tạo từ lịch hẹn (có AppointmentId).</summary>
        public int AppointmentBasedCount { get; set; }

        /// <summary>Số phiếu sửa do user hiện tại (lễ tân) tự tay tạo.</summary>
        public int JobCardsCreatedByMe { get; set; }
    }

    public class AppointmentStatusBreakdown
    {
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int ConvertedToJobCard { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
        public int NoShow { get; set; }
    }
}
