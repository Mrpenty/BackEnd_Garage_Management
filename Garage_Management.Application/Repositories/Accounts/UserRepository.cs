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
        private readonly UserManager<User> userManager;
        private readonly RoleManager<User> roleManager;
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {

        }
        public async Task<bool> CheckPasswordAsync(User user, string password)
        {   
            var res = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (res == null)
            {
                return false;
            }
            var result = await userManager.CheckPasswordAsync(res, password);
            return result;
        }
        public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
        {
            var result = await userManager.CreateAsync(entity, entity.PasswordHash).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Tạo user thất bại: " + string.Join("; ", result.Errors.Select(static e => e.Description)));
            }

            return entity;
        }
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var res = await userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
            return res;
        }
        public async Task<bool> ExistsByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
        {
            var res = await userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
            return res;
        }
        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            var userIdentity = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var result = await userManager.GeneratePasswordResetTokenAsync(userIdentity);
            return result;
        }
        public async Task<string?> GetRoleNameByUser(User user)
        {
            var userIdentity = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }
            var roles = await userManager.GetRolesAsync(userIdentity);
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
            var userIdentity = await userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == user.PhoneNumber);
            if (userIdentity is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var he = await userManager.ResetPasswordAsync(userIdentity, token, newPassword);
            return he.Succeeded;
        }
        public async Task<bool> SetAuthenTokenAsync(User user, string authProvider, string authTokenName, string token)
        {
            var result = await userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (result is null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }

            var setTokenResult = await userManager.SetAuthenticationTokenAsync(result, authProvider, authTokenName, token);
            return setTokenResult.Succeeded;
        }
        public Task<bool> SoftDeleteAsync(int userId, int? deletedBy, CancellationToken cancellationToken = default)
        {
           var user = userManager.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với Id: " + userId);
            }
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = deletedBy;
            var result = userManager.UpdateAsync(user);
            return Task.FromResult(result.Result.Succeeded);
        }
        public Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            user.UpdatedAt = DateTime.UtcNow;
            var result = userManager.UpdateAsync(user);
            return Task.FromResult(result.Result.Succeeded);

        }
        public Task<bool> UpdateRole(User user)
        {
            var userIdentity = userManager.Users.FirstOrDefault(u => u.PhoneNumber == user.PhoneNumber);
            if (userIdentity == null)
            {
                throw new DirectoryNotFoundException("Không tìm thấy user với phone number: " + user.PhoneNumber);
            }
            //userIdentity.RoleId = user.RoleId;
            userIdentity.UpdatedAt = DateTime.UtcNow;
            var result = userManager.UpdateAsync(userIdentity);
            return Task.FromResult(result.Result.Succeeded);
        }
    }
}