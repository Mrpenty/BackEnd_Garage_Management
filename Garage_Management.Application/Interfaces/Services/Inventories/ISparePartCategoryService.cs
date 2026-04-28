using Garage_Management.Application.DTOs.Inventories.SparePartCategories;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    public interface ISparePartCategoryService
    {
        /// <summary>
        /// Lấy danh sách nhóm phụ tùng
        /// </summary>
        /// <param name="query">bao gồm order,filter, search</param>
        /// <param name="onlyActive">hiện được isActive hay không</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<PagedResult<SparePartCategoryResponse>> GetPagedAsync(ParamQuery query, bool onlyActive = false, CancellationToken ct = default);
        Task<SparePartCategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<SparePartCategoryResponse> CreateAsync(SparePartCategoryCreateRequest request, CancellationToken ct = default);
        /// <summary>
        /// Cập nhật name hoặc description của nhóm phụ tùng
        /// có dữ liệu con chỉ được cập nhật description
        /// không có dữ liệu con được cập nhật name và description
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<SparePartCategoryResponse?> UpdateAsync(int id, SparePartCategoryUpdateRequest request, CancellationToken ct = default);
        /// <summary>
        /// cập nhật isActive của nhóm phụ tùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isActive"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<SparePartCategoryResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default);
    }
}
