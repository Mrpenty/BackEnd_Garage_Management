using Garage_Management.Application.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface ITokenCookieService
    {
        void SetTokenCookie(TokenDTO tokenDTO, HttpContext context);
        void DeleteTokenCookie(HttpContext context);
        string? GetAccessToken(HttpContext context);
        string? GetRefreshToken(HttpContext context);
        Task<UserClaimsRequest?> GetUserInfoFromTokenAsync(HttpContext context);
    }
}
