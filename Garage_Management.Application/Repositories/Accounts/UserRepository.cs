using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Repositories.Accounts
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserRepository(AppDbContext dbContext,UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager): base(dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
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
    }
}