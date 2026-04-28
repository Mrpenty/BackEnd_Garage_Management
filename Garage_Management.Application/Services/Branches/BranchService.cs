using Garage_Management.Application.DTOs.Branches;
using Garage_Management.Application.Interfaces.Repositories.Branches;
using Garage_Management.Application.Interfaces.Services.Auth;
using Garage_Management.Application.Interfaces.Services.Branches;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Branches;

namespace Garage_Management.Application.Services.Branches
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _repo;
        private readonly ICurrentUserService _currentUser;

        public BranchService(IBranchRepository repo, ICurrentUserService currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<BranchResponse>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            int? scoped = _currentUser.IsAdmin() ? null : _currentUser.GetCurrentBranchId();
            var paged = await _repo.GetPagedAsync(query, scoped, ct);

            var responses = new List<BranchResponse>();
            foreach (var b in paged.PageData)
            {
                responses.Add(await MapWithCountsAsync(b, ct));
            }

            return new PagedResult<BranchResponse>
            {
                Page = paged.Page,
                PageSize = paged.PageSize,
                Total = paged.Total,
                PageData = responses
            };
        }

        public async Task<BranchResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            EnsureCanAccess(id);
            var entity = await _repo.GetDetailByIdAsync(id, ct);
            return entity == null ? null : await MapWithCountsAsync(entity, ct);
        }

        public async Task<BranchResponse> CreateAsync(BranchCreateRequest request, CancellationToken ct = default)
        {
            if (!_currentUser.IsAdmin())
                throw new UnauthorizedAccessException("Chỉ Admin được tạo chi nhánh");

            var code = (request.BranchCode ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Phải nhập mã chi nhánh");

            var name = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên chi nhánh");

            var address = (request.Address ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidOperationException("Phải nhập địa chỉ chi nhánh");

            if (await _repo.CodeExistsAsync(code, null, ct))
                throw new InvalidOperationException("Mã chi nhánh đã tồn tại");

            var entity = new Branch
            {
                BranchCode = code,
                Name = name,
                Address = address,
                Phone = request.Phone,
                Email = request.Email,
                ManagerEmployeeId = request.ManagerEmployeeId,
                IsActive = request.IsActive,
                CreatedBy = _currentUser.GetCurrentUserId()
            };

            await _repo.AddAsync(entity, ct);
            await _repo.SaveAsync(ct);
            return await MapWithCountsAsync(entity, ct);
        }

        public async Task<BranchResponse?> UpdateAsync(int id, BranchUpdateRequest request, CancellationToken ct = default)
        {
            if (!_currentUser.IsAdmin())
                throw new UnauthorizedAccessException("Chỉ Admin được cập nhật chi nhánh");

            var name = (request.Name ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Phải nhập tên chi nhánh");

            var address = (request.Address ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidOperationException("Phải nhập địa chỉ chi nhánh");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null) return null;

            entity.Name = name;
            entity.Address = address;
            entity.Phone = request.Phone;
            entity.Email = request.Email;
            entity.ManagerEmployeeId = request.ManagerEmployeeId;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = _currentUser.GetCurrentUserId();

            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return await MapWithCountsAsync(entity, ct);
        }

        public async Task<BranchResponse?> UpdateStatusAsync(int id, bool isActive, CancellationToken ct = default)
        {
            if (!_currentUser.IsAdmin())
                throw new UnauthorizedAccessException("Chỉ Admin được đổi trạng thái chi nhánh");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null) return null;

            if (entity.IsActive != isActive)
            {
                entity.IsActive = isActive;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = _currentUser.GetCurrentUserId();
                _repo.Update(entity);
                await _repo.SaveAsync(ct);
            }

            return await MapWithCountsAsync(entity, ct);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            if (!_currentUser.IsAdmin())
                throw new UnauthorizedAccessException("Chỉ Admin được xóa chi nhánh");

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null || entity.DeletedAt != null) return false;

            if (await _repo.HasDependenciesAsync(id, ct))
                throw new InvalidOperationException("Không thể xóa chi nhánh vì còn nhân viên / phụ tùng / phiếu sửa chữa");

            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = _currentUser.GetCurrentUserId();
            _repo.Update(entity);
            await _repo.SaveAsync(ct);
            return true;
        }

        private void EnsureCanAccess(int branchId)
        {
            if (_currentUser.IsAdmin()) return;
            var scoped = _currentUser.GetCurrentBranchId();
            if (scoped.HasValue && scoped.Value == branchId) return;
            throw new UnauthorizedAccessException("Không có quyền truy cập chi nhánh khác");
        }

        private async Task<BranchResponse> MapWithCountsAsync(Branch entity, CancellationToken ct)
        {
            var employeeCount = await _repo.CountEmployeesAsync(entity.BranchId, ct);
            var activeJobCardCount = await _repo.CountActiveJobCardsAsync(entity.BranchId, ct);

            return new BranchResponse
            {
                BranchId = entity.BranchId,
                BranchCode = entity.BranchCode,
                Name = entity.Name,
                Address = entity.Address,
                Phone = entity.Phone,
                Email = entity.Email,
                ManagerEmployeeId = entity.ManagerEmployeeId,
                ManagerEmployeeName = entity.ManagerEmployee == null
                    ? null
                    : $"{entity.ManagerEmployee.LastName} {entity.ManagerEmployee.FirstName}".Trim(),
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                EmployeeCount = employeeCount,
                ActiveJobCardCount = activeJobCardCount
            };
        }
    }
}
