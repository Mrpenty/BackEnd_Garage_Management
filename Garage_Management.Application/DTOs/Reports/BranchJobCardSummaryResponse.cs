using Garage_Management.Base.Common.Enums;

namespace Garage_Management.Application.DTOs.Reports
{
    public class BranchJobCardSummaryResponse
    {
        public int BranchId { get; set; }
        public string BranchCode { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int TotalCount { get; set; }
        public List<JobCardStatusCount> StatusBreakdown { get; set; } = new();
    }

    public class JobCardStatusCount
    {
        public JobCardStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
