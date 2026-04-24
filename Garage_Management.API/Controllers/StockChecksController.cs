using Garage_Management.Application.DTOs.StockChecks;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/stock-checks")]
    // [Authorize(Roles = "Stocker,Supervisor,Admin")]
    public class StockChecksController : ControllerBase
    {
        private readonly IStockCheckService _service;

        public StockChecksController(IStockCheckService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy snapshot tồn kho hệ thống cho phiên kiểm kê (phân trang).
        /// Filter: theo CategoryId / BrandId / list SparePartIds (rỗng = toàn bộ).
        /// Search: theo PartName / PartCode (qua ParamQuery.Search).
        /// </summary>
        [HttpGet("snapshot")]
        public async Task<ActionResult<ApiResponse<PagedResult<StockCheckItemSnapshotResponse>>>> GetSnapshot(
            [FromQuery] ParamQuery query,
            [FromQuery] int? categoryId,
            [FromQuery] int? brandId,
            [FromQuery] List<int>? sparePartIds,
            CancellationToken ct = default)
        {
            var data = await _service.GetSnapshotAsync(query, categoryId, brandId, sparePartIds, ct);
            return Ok(ApiResponse<PagedResult<StockCheckItemSnapshotResponse>>.SuccessResponse(data, "Lấy snapshot tồn kho thành công"));
        }

        /// <summary>
        /// Hoàn tất phiên kiểm kê: tính chênh lệch, tự động tạo Adjustment cho item chênh lệch.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockCheckResultResponse>>> Submit(
            [FromBody] StockCheckSubmitRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.SubmitAsync(request, ct);
                return Ok(ApiResponse<StockCheckResultResponse>.SuccessResponse(
                    data,
                    $"Stock check completed successfully. {data.DiscrepanciesAdjusted} discrepancies adjusted."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<StockCheckResultResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<StockCheckResultResponse>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Lấy danh sách các phiên kiểm kê đã thực hiện (phân trang, search theo ReceiptCode, filter ngày).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StockCheckSessionResponse>>>> GetPagedSessions(
            [FromQuery] ParamQuery query,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedSessionsAsync(query, from, to, ct);
            return Ok(ApiResponse<PagedResult<StockCheckSessionResponse>>.SuccessResponse(data, "Lấy danh sách phiên kiểm kê thành công"));
        }

        /// <summary>
        /// Truy vấn chi tiết 1 phiên kiểm kê đã thực hiện theo ReceiptCode.
        /// </summary>
        [HttpGet("{receiptCode}")]
        public async Task<ActionResult<ApiResponse<StockCheckSessionResponse>>> GetByReceiptCode(
            string receiptCode,
            CancellationToken ct = default)
        {
            var data = await _service.GetByReceiptCodeAsync(receiptCode, ct);
            if (data == null)
                return NotFound(ApiResponse<StockCheckSessionResponse>.ErrorResponse("Không tìm thấy phiên kiểm kê"));

            return Ok(ApiResponse<StockCheckSessionResponse>.SuccessResponse(data, "OK"));
        }
    }
}
