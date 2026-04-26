using Garage_Management.Application.DTOs.Inventories.Suppliers;
using Garage_Management.Base.Common.Models;

namespace Garage_Management.Application.Interfaces.Services.Inventories
{
    public interface ISupplierService
    {
        Task<PagedResult<SupplierResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);
        Task<SupplierResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<SupplierResponse> CreateAsync(SupplierCreateRequest request, CancellationToken ct = default);
        Task<SupplierResponse?> UpdateAsync(int id, SupplierUpdateRequest request, CancellationToken ct = default);
        Task<SupplierResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default);
    }
}
