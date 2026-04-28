using Garage_Management.Application.DTOs.StockChecks;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    public interface IStockCheckService
    {
        /// <summary>
        /// Lấy snapshot tồn kho hệ thống cho phiên kiểm kê (phân trang, lọc theo category/brand/SparePartIds).
        /// </summary>
        Task<PagedResult<StockCheckItemSnapshotResponse>> GetSnapshotAsync(
            ParamQuery query,
            int? categoryId,
            int? brandId,
            List<int>? sparePartIds,
            CancellationToken ct = default);

        /// <summary>
        /// Hoàn tất phiên kiểm kê: tính delta, tự động tạo StockTransaction Adjustment cho mỗi item có chênh lệch.
        /// </summary>
        Task<StockCheckResultResponse> SubmitAsync(StockCheckSubmitRequest request, CancellationToken ct = default);

        /// <summary>
        /// Truy vấn chi tiết 1 phiên kiểm kê đã hoàn tất theo ReceiptCode.
        /// </summary>
        Task<StockCheckSessionResponse?> GetByReceiptCodeAsync(string receiptCode, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách các phiên kiểm kê đã thực hiện (phân trang, search, filter).
        /// </summary>
        Task<PagedResult<StockCheckSessionResponse>> GetPagedSessionsAsync(
            ParamQuery query,
            DateTime? from,
            DateTime? to,
            CancellationToken ct = default);
    }
}
