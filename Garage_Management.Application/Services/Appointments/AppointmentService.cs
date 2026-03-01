using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Repositories.Appointments;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Repositories.Services;
using Garage_Management.Application.Interfaces.Repositories.Vehiclies;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using Garage_Management.Base.Common.Models.Appointments;

namespace Garage_Management.Application.Services.Appointments
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IServiceRepository _serviceRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentService(
            IAppointmentRepository repo,
            IEmployeeRepository employeeRepo,
            ICustomerRepository customerRepo,
            IVehicleRepository vehicleRepo,
            IServiceRepository serviceRepo,
            IInventoryRepository inventoryRepo,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
            _customerRepo = customerRepo;
            _vehicleRepo = vehicleRepo;
            _serviceRepo = serviceRepo;
            _inventoryRepo = inventoryRepo;
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

        public async Task<PagedResult<AppointmentResponse>> GetPagedAsync(AppointmentQuery query, CancellationToken ct = default)
        {
            var paged = await _repo.GetPagedAsync(query, ct);
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
            if (request.CustomerId.HasValue && request.CustomerId.Value <= 0)
                throw new InvalidOperationException("CustomerId không hợp lệ");

            if (request.CustomerId.HasValue)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName) ||
                    !string.IsNullOrWhiteSpace(request.LastName) ||
                    !string.IsNullOrWhiteSpace(request.Phone))
                {
                    throw new InvalidOperationException("Có CustomerId thì không được nhập FirstName/LastName/Phone");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) ||
                    string.IsNullOrWhiteSpace(request.LastName) ||
                    string.IsNullOrWhiteSpace(request.Phone))
                {
                    throw new InvalidOperationException("Khách vãng lai cần FirstName, LastName và Phone");
                }
            }

            if (request.CustomerId.HasValue)
            {
                var customer = await _customerRepo.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                    throw new InvalidOperationException("CustomerId không tồn tại");
            }

            var hasVehicleId = request.VehicleId.HasValue;
            var hasVehicleModel = request.VehicleModelId.HasValue;

            if (hasVehicleId != hasVehicleModel)
                throw new InvalidOperationException("VehicleId và VehicleModelId phải có cùng lúc");

            if (hasVehicleId)
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId.Value);
                if (vehicle == null)
                    throw new InvalidOperationException("VehicleId không tồn tại");
            }

            var serviceIds = request.ServiceIds?
                .Where(x => x > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (serviceIds.Count > 0)
            {
                var count = await _serviceRepo.GetAll()
                    .AsNoTracking()
                    .CountAsync(x => serviceIds.Contains(x.ServiceId), ct);
                if (count != serviceIds.Count)
                    throw new InvalidOperationException("ServiceIds không hợp lệ");
            }

            var sparePartIds = request.SparePartsIds?
                .Where(x => x > 0)
                .Distinct()
                .ToList() ?? new List<int>();

            if (sparePartIds.Count > 0)
            {
                var count = await _inventoryRepo.Query()
                    .AsNoTracking()
                    .CountAsync(x => sparePartIds.Contains(x.SparePartId), ct);
                if (count != sparePartIds.Count)
                    throw new InvalidOperationException("SparePartsIds không hợp lệ");
            }

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
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                VehicleId = request.VehicleId,
                VehicleModelId = request.VehicleModelId,
                CreatedBy = createdBy,
                AppointmentDateTime = request.AppointmentDateTime,
                Status = request.Status,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            if (serviceIds.Count > 0)
            {
                entity.Services = serviceIds
                    .Select(id => new Base.Entities.Accounts.AppointmentService
                    {
                        ServiceId = id
                    })
                    .ToList();
            }

            if (sparePartIds.Count > 0)
            {
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

            if (request.CustomerId.HasValue)
            {
                if (request.CustomerId.Value <= 0)
                    throw new InvalidOperationException("CustomerId không hợp lệ");
                var customer = await _customerRepo.GetByIdAsync(request.CustomerId.Value);
                if (customer == null)
                    throw new InvalidOperationException("CustomerId không tồn tại");
                entity.CustomerId = request.CustomerId.Value;
            }
            if (request.FirstName != null)
            {
                if (string.IsNullOrWhiteSpace(request.FirstName))
                    throw new InvalidOperationException("FirstName không hợp lệ");
                if (request.CustomerId.HasValue)
                    throw new InvalidOperationException("Có CustomerId thì không được nhập FirstName");
                entity.FirstName = request.FirstName;
            }
            if (request.LastName != null)
            {
                if (string.IsNullOrWhiteSpace(request.LastName))
                    throw new InvalidOperationException("LastName không hợp lệ");
                if (request.CustomerId.HasValue)
                    throw new InvalidOperationException("Có CustomerId thì không được nhập LastName");
                entity.LastName = request.LastName;
            }
            if (request.Phone != null)
            {
                if (string.IsNullOrWhiteSpace(request.Phone))
                    throw new InvalidOperationException("Phone không hợp lệ");
                if (request.CustomerId.HasValue)
                    throw new InvalidOperationException("Có CustomerId thì không được nhập Phone");
                entity.Phone = request.Phone;
            }
            if (!request.CustomerId.HasValue)
            {
                if (string.IsNullOrWhiteSpace(entity.FirstName) ||
                    string.IsNullOrWhiteSpace(entity.LastName) ||
                    string.IsNullOrWhiteSpace(entity.Phone))
                {
                    throw new InvalidOperationException("Khách vãng lai cần FirstName, LastName và Phone");
                }
            }
            if (request.VehicleId.HasValue)
            {
                var vehicle = await _vehicleRepo.GetByIdAsync(request.VehicleId.Value);
                if (vehicle == null)
                    throw new InvalidOperationException("VehicleId không tồn tại");
                entity.VehicleId = request.VehicleId.Value;
            }
            if (request.VehicleModelId.HasValue)
                entity.VehicleModelId = request.VehicleModelId.Value;

            var hasVehicleIdUpdate = entity.VehicleId.HasValue;
            var hasVehicleModelUpdate = entity.VehicleModelId.HasValue;
            if (hasVehicleIdUpdate != hasVehicleModelUpdate)
                throw new InvalidOperationException("VehicleId và VehicleModelId phải có cùng lúc");
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
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Phone = entity.Phone,
                VehicleId = entity.VehicleId,
                VehicleModelId = entity.VehicleModelId,
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
