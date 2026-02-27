using Garage_Management.Application.DTOs.User;
using Garage_Management.Base.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Lấy thông tin profile người dùng hiện tại
        /// </summary>
        Task<ApiResponse<ProfileResponse>> GetCurrentUserProfileAsync(ClaimsPrincipal userClaims);
    }
}
