using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.Accounts
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext dbContext;

        public UserRepository(AppDbContext dbContext, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, ICustomerRepository customerRepository, IEmployeeRepository employeeRepository) : base(dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            this.dbContext = dbContext;
        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            ArgumentNullException.ThrowIfNull(user);
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(entity, entity.PasswordHash).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Tạo user thất bại: " + string.Join("; ", result.Errors.Select(static e => e.Description)));
            }

            return entity;
        }
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var res = await _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
            return res;
        }
        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var res = await _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
            return res;
        }
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var result = await _userManager.GeneratePasswordResetTokenAsync(userIdentity);
            return result;
        }
        public async Task<string?> GetRoleNameByUser(User user)
        {
            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }
            var roles = await _userManager.GetRolesAsync(userIdentity);
            return roles.FirstOrDefault();
        }
        public Task<User?> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }
        public Task RemoveCached(User user)
        {
            return Task.CompletedTask;
        }
        public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var userIdentity = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var he = await _userManager.ResetPasswordAsync(userIdentity, token, newPassword);
            return he.Succeeded;
        }
        public async Task<bool> SetAuthenTokenAsync(User user, string authProvider, string authTokenName, string token)
        {
            var result = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (result is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var setTokenResult = await _userManager.SetAuthenticationTokenAsync(result, authProvider, authTokenName, token);
            return setTokenResult.Succeeded;
        }
        public Task<bool> SoftDeleteAsync(int userId, int? deletedBy, CancellationToken cancellationToken = default)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với Id: " + userId);
            }
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = deletedBy;
            var result = _userManager.UpdateAsync(user);
            return Task.FromResult(result.Result.Succeeded);
        }
        public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.UpdatedAt = DateTime.UtcNow;
            var result = _userManager.UpdateAsync(user);
            return Task.FromResult(result.Result.Succeeded);

        }
        public async Task<bool> UpdateRole(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var role = await _roleManager.FindByNameAsync("Customer");
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<PagedResult<User>> GetPagedAsync(ParamQuery query, CancellationToken ct = default)
        {
            var q = dbSet.AsNoTracking().AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim().ToLower();
                q = q.Where(u =>
                    (u.UserName ?? "").ToLower().Contains(search) ||
                    (u.Email ?? "").ToLower().Contains(search) ||
                    (u.PhoneNumber ?? "").ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(query.Filter))
            {
                var roleNames = query.Filter.Trim()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(r => r.Trim().ToLower())
                    .ToList();

                if (roleNames.Any())
                {
                    var roleIds = await _roleManager.Roles
                        .Where(r => roleNames.Contains(r.Name.ToLower()))
                        .Select(r => r.Id)
                        .ToListAsync(ct);

                    if (roleIds.Any())
                    {
                        q = q.Where(u => dbContext.UserRoles.Any(ur => ur.UserId == u.Id && roleIds.Contains(ur.RoleId)));
                    }
                    else
                    {
                        q = q.Where(u => false);
                    }
                }
            }
            q = q.OrderByDescending(u => u.CreatedAt);

            var total = await q.CountAsync(ct);

            var data = await q
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(ct);

            return new PagedResult<User>
            {
                PageData = data,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<List<string>> GetUserRolesAsync(int userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return (List<string>)(user != null ? await _userManager.GetRolesAsync(user) : new List<string>());
        }

        public async Task<List<User>> GetCustomersAsync(CancellationToken ct = default)
        {
            var customerUserIds = await _customerRepository.GetAll()
                .Include(x=> x.User)
                .Select(c => c.UserId)
                .ToListAsync(ct);
            var customers = await dbSet
                .Where(u => customerUserIds.Contains(u.Id))
                .ToListAsync(ct);
            return customers;
        }

        public async Task<List<User>> GetEmployeesAsync(CancellationToken ct = default)
        {
            var EmployeeUserIds = await _employeeRepository.GetAll()
                .Select(e => e.UserId)
                .ToListAsync(ct);
            var employees = await dbSet
                .Where(u => EmployeeUserIds.Contains(u.Id))
                .ToListAsync(ct);
            return employees;
        }
    }
}