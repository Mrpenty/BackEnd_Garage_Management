using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    /// <summary>
    /// Service nghiệp vụ quản lý danh mục phụ tùng trong kho.
    /// Tất cả method check branch đều nhận branchId từ FE (không lấy từ token).
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Lấy danh sách phụ tùng có phân trang, tìm kiếm, lọc và sắp xếp. branchId optional — null = không filter branch.
        /// </summary>
        Task<ApiResponse<PagedResult<InventoryResponse>>> GetPagedAsync(ParamQuery query, int? branchId = null, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách phụ tùng theo BranchId được FE truyền vào.
        /// </summary>
        Task<ApiResponse<PagedResult<InventoryResponse>>> GetByBranchIdAsync(int branchId, ParamQuery query, CancellationToken ct = default);

        /// <summary>
        /// Lấy chi tiết 1 phụ tùng theo mã. branchId bắt buộc — phải khớp branch của entity.
        /// </summary>
        Task<InventoryResponse?> GetByIdAsync(int id, int branchId, CancellationToken ct = default);

        /// <summary>
        /// Tạo mới một phụ tùng trong kho. BranchId bắt buộc trong request.
        /// </summary>
        Task<InventoryResponse> CreateAsync(InventoryCreateRequest request, CancellationToken ct = default);

        /// <summary>
        /// Cập nhật thông tin phụ tùng. branchId bắt buộc — phải khớp branch của entity.
        /// </summary>
        Task<InventoryResponse?> UpdateAsync(int id, int branchId, InventoryUpdateRequest request, CancellationToken ct = default);

        /// <summary>
        /// Cập nhật trạng thái hoạt động của phụ tùng. branchId bắt buộc.
        /// </summary>
        Task<InventoryResponse?> UpdateStatusAsync(int id, int branchId, bool isActive, CancellationToken ct = default);

        /// <summary>
        /// Xóa cứng phụ tùng. branchId bắt buộc.
        /// </summary>
        Task<bool> DeleteAsync(int id, int branchId, CancellationToken ct = default);

        /// <summary>
        /// Lấy danh sách phụ tùng theo hãng. branchId optional.
        /// </summary>
        Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, int? branchId = null, CancellationToken ct = default);
    }
}
