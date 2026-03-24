using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    /// <summary>
    /// Service nghiệp vụ giao dịch kho.
    /// Bao gồm các thao tác nhập kho, xuất kho và truy vấn lịch sử biến động tồn.
    /// </summary>
    public interface IStockTransactionService
    {
        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Lấy danh sách giao dịch kho có phân trang, tìm kiếm, lọc và sắp xếp.
        /// </summary>
        /// <param name="query">Tham số phân trang và điều kiện truy vấn.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>PagedResult chứa danh sách giao dịch kho.</returns>
        Task<PagedResult<StockTransactionResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Lấy chi tiết 1 giao dịch kho theo mã.
        /// </summary>
        /// <param name="id">Mã giao dịch kho (TransactionId).</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>StockTransactionResponse nếu tồn tại; ngược lại trả về null.</returns>
        Task<StockTransactionResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Tạo mới giao dịch kho và cập nhật tồn kho tương ứng cho phụ tùng.
        /// </summary>
        /// <param name="request">Dữ liệu tạo giao dịch kho.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>Thông tin giao dịch kho vừa tạo.</returns>
        Task<StockTransactionResponse> CreateAsync(StockTransactionCreateRequest request, CancellationToken ct = default);
    }
}
