using Garage_Management.Application.DTOs.Reports;

namespace Garage_Management.Application.Interfaces.Services.Reports
{
    public interface IReportService
    {
        Task<BranchRevenueResponse?> GetBranchRevenueAsync(int branchId, DateTime? from, DateTime? to, CancellationToken ct = default);
        Task<BranchJobCardSummaryResponse?> GetBranchJobCardSummaryAsync(int branchId, DateTime? from, DateTime? to, CancellationToken ct = default);
        Task<List<BranchRevenueResponse>> GetRevenueByBranchAsync(DateTime? from, DateTime? to, CancellationToken ct = default);
    }
}
