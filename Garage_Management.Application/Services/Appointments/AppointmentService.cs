using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System;
using System.Linq;

namespace Garage_Management.Application.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentService(IAppointmentRepository repo, IEmployeeRepository employeeRepo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppointmentResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdWithDetailsAsync(id, ct);
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
            int? createdBy = null;
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(userIdStr) && int.TryParse(userIdStr, out var userId))
            {
                var employee = await _employeeRepo.GetByUserIdAsync(userId);
                createdBy = employee?.EmployeeId;
            }

            var entity = new Appointment
            {
                CustomerId = request.CustomerId,
                VehicleId = request.VehicleId,
                CreatedBy = createdBy,
                AppointmentDateTime = request.AppointmentDateTime,
                Status = request.Status,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            if (request.ServiceIds != null && request.ServiceIds.Count > 0)
            {
                var serviceIds = request.ServiceIds
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                entity.Services = serviceIds
                    .Select(id => new Base.Entities.Accounts.AppointmentService
                    {
                        ServiceId = id
                    })
                    .ToList();
            }

            if (request.SparePartsIds != null && request.SparePartsIds.Count > 0)
            {
                var sparePartIds = request.SparePartsIds
                    .Where(x => x > 0)
                    .Distinct()
                    .ToList();

                entity.SpareParts = sparePartIds
                    .Select(id => new AppointmentSparePart
                    {
                        SparePartId = id
                    })
                    .ToList();
            }

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);

            var detail = await _repo.GetByIdWithDetailsAsync(entity.AppointmentId, ct);
            return detail == null ? Map(entity) : Map(detail);
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

            var detail = await _repo.GetByIdWithDetailsAsync(id, ct);
            return detail == null ? Map(entity) : Map(detail);
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
                TotalEstimateMinute = entity.Services.Sum(s => s.Service?.ServiceTasks.Sum(t => (long)t.EstimateMinute) ?? 0),
                CreatedBy = entity.CreatedBy,
                UpdatedBy = entity.UpdatedBy,
                Services = entity.Services
                    .Where(x => x.Service != null)
                    .Select(x => new ServiceResponse
                    {
                        ServiceId = x.Service.ServiceId,
                        ServiceName = x.Service.ServiceName,
                        BasePrice = x.Service.BasePrice,
                        Description = x.Service.Description,
                        TotalEstimateMinute = x.Service.ServiceTasks.Sum(t => (long)t.EstimateMinute),
                        ServiceTasks = x.Service.ServiceTasks
                            .OrderBy(t => t.TaskOrder)
                            .Select(t => new ServiceTaskResponse
                            {
                                ServiceTaskId = t.ServiceTaskId,
                                ServiceId = t.ServiceId,
                                TaskName = t.TaskName,
                                TaskOrder = t.TaskOrder,
                                EstimateMinute = t.EstimateMinute,
                                Note = t.Note,
                                CreatedAt = t.CreatedAt,
                                UpdatedAt = t.UpdatedAt
                            })
                            .ToList(),
                        IsActive = x.Service.IsActive,
                        CreatedAt = x.Service.CreatedAt,
                        UpdatedAt = x.Service.UpdatedAt
                    })
                    .ToList(),
                SpareParts = entity.SpareParts
                    .Where(x => x.Inventory != null)
                    .Select(x => new InventoryResponse
                    {
                        SparePartId = x.Inventory.SparePartId,
                        PartName = x.Inventory.PartName,
                        Unit = x.Inventory.Unit,
                        SparePartBrandId = x.Inventory.SparePartBrandId,
                        SparePartBrandName = x.Inventory.SparePartBrand != null ? x.Inventory.SparePartBrand.BrandName : null,
                        LastPurchasePrice = x.Inventory.LastPurchasePrice,
                        ModelCompatible = x.Inventory.ModelCompatible,
                        VehicleBrand = x.Inventory.VehicleBrand,
                        SellingPrice = x.Inventory.SellingPrice,
                        IsActive = x.Inventory.IsActive
                    })
                    .ToList(),
                AppointmentDateTime = entity.AppointmentDateTime,
                Status = entity.Status,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
