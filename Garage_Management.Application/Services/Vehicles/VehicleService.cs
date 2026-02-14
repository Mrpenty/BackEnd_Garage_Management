using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.DTOs.Vehicles;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Vehicles
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repo;

        public VehicleService(IVehicleRepository repo)
        {
            _repo = repo;
        }

        public async Task<VehicleResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<VehicleResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default)
        {
            var data = await _repo.GetByCustomerIdAsync(page, pageSize, customerId, ct);
            return new PagedResult<VehicleResponse>
            {
                Page = data.Page,
                PageSize = data.PageSize,
                Total = data.Total,
                PageData = data.PageData.Select(Map).ToList()
            };
        }

        public async Task<PagedResult<VehicleResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<VehicleResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<VehicleResponse> CreateAsync(VehicleCreateRequest request, CancellationToken ct = default)
        {
            var entity = new Vehicle
            {
                CustomerId = request.CustomerId,
                BrandId = request.BrandId,
                ModelId = request.ModelId,
                LicensePlate = request.LicensePlate,
                Year = request.Year,
                Vin = request.Vin,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<VehicleResponse?> UpdateAsync(int id, VehicleUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.BrandId.HasValue) entity.BrandId = request.BrandId.Value;
            if (request.ModelId.HasValue) entity.ModelId = request.ModelId.Value;
            if (request.LicensePlate != null) entity.LicensePlate = request.LicensePlate;
            if (request.Year.HasValue) entity.Year = request.Year.Value;
            if (request.Vin != null) entity.Vin = request.Vin;
            if (request.UpdatedBy.HasValue) entity.UpdatedBy = request.UpdatedBy.Value;
            entity.UpdatedAt = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Delete(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private static VehicleResponse Map(Vehicle entity)
        {
            return new VehicleResponse
            {
                VehicleId = entity.VehicleId,
                CustomerId = entity.CustomerId,
                BrandId = entity.BrandId,
                ModelId = entity.ModelId,
                LicensePlate = entity.LicensePlate,
                Year = entity.Year,
                Vin = entity.Vin,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
