using Garage_Management.Application.DTOs.Auth;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Services.Accounts
{
    public class TokenCookieService : ITokenCookieService
    {
        private readonly JwtConfiguration jwtConfig;

        private const string AccessTokenCookieName = "accessToken";
        private const string RefreshTokenCookieName = "refreshToken";
        private readonly IConfiguration _configuration;

        public TokenCookieService(IOptions<JwtConfiguration> jwtConfig, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(jwtConfig);
            this.jwtConfig = jwtConfig.Value;
            _configuration = configuration;
        }
        public void SetTokenCookie(TokenDTO tokenDTO, HttpContext context)
        {
            context.Response.Cookies.Append("accessToken", tokenDTO.AccessToken,
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(3),
                        HttpOnly = true,
                        IsEssential = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax
                    }
                );

            context.Response.Cookies.Append("refreshToken", tokenDTO.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                }
            );
        }
        public void DeleteTokenCookie(HttpContext context)
        {
            var expiredOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };

            context.Response.Cookies.Delete(AccessTokenCookieName, expiredOptions);
            context.Response.Cookies.Delete(RefreshTokenCookieName, expiredOptions);
        }

        public string? GetAccessToken(HttpContext context)
        {
            // Ưu tiên cookie
            if (context.Request.Cookies.TryGetValue(AccessTokenCookieName, out var cookieToken)
                && !string.IsNullOrWhiteSpace(cookieToken))
            {
                return cookieToken;
            }

            // Fallback: Authorization header Bearer
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
        public string? GetRefreshToken(HttpContext context)
        {
            return context.Request.Cookies[RefreshTokenCookieName];
        }
        public async Task<UserClaimsRequest?> GetUserInfoFromTokenAsync(HttpContext context)
        {
            var token = GetAccessToken(context);
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key missing"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero // optional: không cho phép lệch giờ
                };

                // Validate token và lấy claims
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                if (principal == null || validatedToken is not JwtSecurityToken jwtToken)
                {
                    return null;
                }

                var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdStr, out var userId))
                {
                    return null;
                }

                var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

                return new UserClaimsRequest    
                {
                    UserId = userId,
                    Email = principal.FindFirst(ClaimTypes.Email)?.Value,
                    UserName = principal.FindFirst(ClaimTypes.Name)?.Value,
                    // Nếu bạn lưu PhoneNumber trong claims thì thêm vào đây
                    // PhoneNumber = principal.FindFirst("phone_number")?.Value,
                    Roles = roles,
                    IsAuthenticated = true,
                    TokenSource = context.Request.Cookies.ContainsKey(AccessTokenCookieName) ? "cookie" : "header"
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
