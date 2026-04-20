using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Data;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Services
{
    public class GenerateToken : IGenerateToken
    {
        private readonly JwtConfiguration jwtConfig;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _dbContext;

        public GenerateToken(IOptions<JwtConfiguration> jwtConfig, UserManager<User> userManager, AppDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(jwtConfig);


            this.jwtConfig = jwtConfig.Value;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            // Support either a base64-encoded secret or a plain text secret
            var secret = this.jwtConfig.Key ?? throw new InvalidOperationException("JWT Key is missing at runtime.");
            byte[] keyBytes;
            try
            {
                keyBytes = Convert.FromBase64String(secret);
            }
            catch (FormatException)
            {
                keyBytes = Encoding.UTF8.GetBytes(secret);
            }

            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(ClaimTypes.Email, user.Email),
            };
            //const string issuer = "MGMS.API";
            //const string audience = "MGMS.Client";

            var roles = await _userManager.GetRolesAsync(user); 
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Thêm CustomerId nếu user là Customer
            var customer = await _dbContext.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (customer != null)
            {
                claims.Add(new Claim("CustomerId", customer.CustomerId.ToString()));
            }

            // Thêm EmployeeId nếu user là nhân viên (giả sử bạn có bảng Employees)
            var employee = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.UserId == user.Id);
            if (employee != null)
            {
                claims.Add(new Claim("EmployeeId", employee.EmployeeId.ToString()));
                claims.Add(new Claim("BranchId", employee.BranchId.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: jwtConfig.Issuer ?? "MGMS.API",
                audience: jwtConfig.Audience ?? "MGMS.Client",
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public string GetAuthProvider() => this.jwtConfig.AuthProvider;

        public string GetAuthToken() => this.jwtConfig.AuthTokenName;

        public int GetExpireToken() => 60;

       
    }
}
