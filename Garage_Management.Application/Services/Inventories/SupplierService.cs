using Garage_Management.Application.DTOs.Inventories.Suppliers;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Inventories;

namespace Garage_Management.Application.Services.Inventories
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;

        public SupplierService(ISupplierRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<SupplierResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(query, ct);
            return new PagedResult<SupplierResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<SupplierResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<SupplierResponse> CreateAsync(SupplierCreateRequest request, CancellationToken ct = default)
        {
            var name = (request.SupplierName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên cho nhà cung cấp");

            if (!Enum.IsDefined(typeof(Base.Common.Enums.SupplierType), request.SupplierType))
                throw new InvalidOperationException("SupplierType không hợp lệ");

            if (await _repo.HasExistAsync(name, null, ct))
                throw new InvalidOperationException("Nhà cung cấp đã tồn tại");

            var entity = new Supplier
            {
                SupplierName = name,
                SupplierType = request.SupplierType,
                Phone = request.Phone,
                Address = request.Address,
                TaxCode = request.TaxCode,
                IsActive = request.IsActive,
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SupplierResponse?> UpdateAsync(int id, SupplierUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            var name = (request.SupplierName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên cho nhà cung cấp");

            if (await _repo.HasExistAsync(name, entity.SupplierId, ct))
                throw new InvalidOperationException("Nhà cung cấp đã tồn tại");

            entity.SupplierName = name;
            entity.SupplierType = request.SupplierType;
            entity.Phone = request.Phone;
            entity.Address = request.Address;
            entity.TaxCode = request.TaxCode;
            //Không được update isAcitve, đã có api riêng để cập nhật isActive

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<SupplierResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (entity.IsActive != isActive)
            {
                entity.IsActive = isActive;
                _repo.Update(entity);
                await _repo.SaveAsync(ct);
            }

            return Map(entity);
        }

        private static SupplierResponse Map(Supplier entity)
        {
            return new SupplierResponse
            {
                SupplierId = entity.SupplierId,
                SupplierName = entity.SupplierName,
                SupplierType = entity.SupplierType,
                Phone = entity.Phone,
                Address = entity.Address,
                TaxCode = entity.TaxCode,
                IsActive = entity.IsActive
            };
        }
    }
}
