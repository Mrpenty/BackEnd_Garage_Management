using Garage_Management.Application.DTOs.Reports;
using Garage_Management.Application.Interfaces.Services.Reports;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private const string BranchReportRoles = "Admin,Supervisor";
        private const string AdminOnly = "Admin";

        private readonly IReportService _service;

        public ReportsController(IReportService service)
        {
            _service = service;
        }

        [HttpGet("branches/{branchId:int}/revenue")]
        [Authorize(Roles = BranchReportRoles)]
        public async Task<ActionResult<ApiResponse<BranchRevenueResponse>>> GetBranchRevenue(
            int branchId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct = default)
        {
            var data = await _service.GetBranchRevenueAsync(branchId, from, to, ct);
            if (data == null)
                return NotFound(ApiResponse<BranchRevenueResponse>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<BranchRevenueResponse>.SuccessResponse(data, "OK"));
        }

        [HttpGet("branches/{branchId:int}/jobcards")]
        [Authorize(Roles = BranchReportRoles)]
        public async Task<ActionResult<ApiResponse<BranchJobCardSummaryResponse>>> GetBranchJobCardSummary(
            int branchId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct = default)
        {
            var data = await _service.GetBranchJobCardSummaryAsync(branchId, from, to, ct);
            if (data == null)
                return NotFound(ApiResponse<BranchJobCardSummaryResponse>.ErrorResponse("Không tìm thấy chi nhánh"));

            return Ok(ApiResponse<BranchJobCardSummaryResponse>.SuccessResponse(data, "OK"));
        }

        [HttpGet("revenue-by-branch")]
        [Authorize(Roles = AdminOnly)]
        public async Task<ActionResult<ApiResponse<List<BranchRevenueResponse>>>> GetRevenueByBranch(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct = default)
        {
            var data = await _service.GetRevenueByBranchAsync(from, to, ct);
            return Ok(ApiResponse<List<BranchRevenueResponse>>.SuccessResponse(data, "OK"));
        }
    }
}
