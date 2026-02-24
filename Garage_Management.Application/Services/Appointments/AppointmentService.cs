using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
        }

        public async Task<AppointmentResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : Map(entity);
        }

        public async Task<PagedResult<AppointmentResponse>> GetByCustomerIdAsync(int page, int pageSize, int customerId, CancellationToken ct = default)
        {
            var data = await _repo.GetByCustomerIdAsync(page, pageSize, customerId, ct);
            return new PagedResult<AppointmentResponse>
            {
                Page = data.Page,
                PageSize = data.PageSize,
                Total = data.Total,
                PageData = data.PageData.Select(Map).ToList()
            };
        }

        public async Task<PagedResult<AppointmentResponse>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(page, pageSize, ct);
            return new PagedResult<AppointmentResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = paged.PageData.Select(Map).ToList()
            };
        }

        public async Task<AppointmentResponse> CreateAsync(AppointmentCreateRequest request, CancellationToken ct = default)
        {
            var entity = new Appointment
            {
                CustomerId = request.CustomerId,
                VehicleId = request.VehicleId,
                CreatedBy = request.CreatedBy,
                AppointmentDateTime = request.AppointmentDateTime,
                Status = request.Status,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return Map(entity);
        }

        public async Task<AppointmentResponse?> UpdateAsync(int id, AppointmentUpdateRequest request, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.CustomerId.HasValue) entity.CustomerId = request.CustomerId.Value;
            if (request.VehicleId.HasValue) entity.VehicleId = request.VehicleId.Value;
            if (request.AppointmentDateTime.HasValue) entity.AppointmentDateTime = request.AppointmentDateTime.Value;
            if (request.Status.HasValue) entity.Status = request.Status.Value;
            if (request.Description != null) entity.Description = request.Description;
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

        private static AppointmentResponse Map(Appointment entity)
        {
            return new AppointmentResponse
            {
                AppointmentId = entity.AppointmentId,
                CustomerId = entity.CustomerId,
                VehicleId = entity.VehicleId,
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                AppointmentDateTime = entity.AppointmentDateTime,
                Status = entity.Status,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
