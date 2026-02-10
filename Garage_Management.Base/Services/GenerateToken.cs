using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Services
{
    public class GenerateToken : IGenerateToken
    {
        private readonly JwtConfiguration jwtConfig;

        public GenerateToken(IOptions<JwtConfiguration> jwtConfig)
        {
            ArgumentNullException.ThrowIfNull(jwtConfig);

            this.jwtConfig = jwtConfig.Value;
        }

        public string GenerateJwtToken(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtConfig.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, user.Username ?? string.Empty),
                new(ClaimTypes.Email, user.Email),
            };
            const string issuer = "MGMS.API";
            const string audience = "MGMS.Client";

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddYears(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetAuthProvider() => this.jwtConfig.AuthProvider;

        public string GetAuthToken() => this.jwtConfig.AuthTokenName;

        public int GetExpireToken() => 60;
    }
}