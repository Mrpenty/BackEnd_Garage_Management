using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    /// <summary>
    /// Service nghiệp vụ quản lý danh mục phụ tùng trong kho.
    /// Bao gồm: lấy danh sách, chi tiết, tạo mới, cập nhật, đổi trạng thái và xóa.
    /// </summary>
    public interface IInventoryService
    {
        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Lấy danh sách phụ tùng có phân trang, tìm kiếm, lọc và sắp xếp.
        /// </summary>
        /// <param name="query">Tham số phân trang và điều kiện truy vấn.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>ApiResponse chứa PagedResult của InventoryResponse.</returns>
        Task<ApiResponse<PagedResult<InventoryResponse>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Lấy chi tiết 1 phụ tùng theo mã.
        /// </summary>
        /// <param name="id">Mã định danh phụ tùng (SparePartId).</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>InventoryResponse nếu tồn tại, ngược lại trả về null.</returns>
        Task<InventoryResponse?> GetByIdAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Tạo mới một phụ tùng trong kho.
        /// </summary>
        /// <param name="request">Dữ liệu tạo phụ tùng.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>Thông tin phụ tùng vừa được tạo.</returns>
        Task<InventoryResponse> CreateAsync(InventoryCreateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Cập nhật thông tin phụ tùng.
        /// </summary>
        /// <param name="id">Mã định danh phụ tùng cần cập nhật.</param>
        /// <param name="request">Dữ liệu cập nhật.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>InventoryResponse sau cập nhật; null nếu không tìm thấy.</returns>
        Task<InventoryResponse?> UpdateAsync(int id, InventoryUpdateRequest request, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Cập nhật trạng thái hoạt động của phụ tùng.
        /// </summary>
        /// <param name="id">Mã định danh phụ tùng cần đổi trạng thái.</param>
        /// <param name="isActive">Trạng thái mới: true = hoạt động, false = ngừng hoạt động.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>InventoryResponse sau cập nhật; null nếu không tìm thấy.</returns>
        Task<InventoryResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Xóa cứng phụ tùng (chỉ khi không còn dữ liệu phát sinh liên quan).
        /// </summary>
        /// <param name="id">Mã định danh phụ tùng cần xóa.</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>True nếu xóa thành công; false nếu không tìm thấy.</returns>
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);

        /// Author: KhanhDV
        /// Created Date: 24-03-2026
        /// <summary>
        /// Lấy danh sách phụ tùng theo hãng phụ tùng.
        /// </summary>
        /// <param name="brandId">Mã hãng phụ tùng (SparePartBrandId).</param>
        /// <param name="ct">Token hủy tác vụ bất đồng bộ.</param>
        /// <returns>Danh sách phụ tùng thuộc hãng tương ứng.</returns>
        Task<List<InventoryResponse>> GetByBrandIdAsync(int brandId, CancellationToken ct = default);
    }
}
