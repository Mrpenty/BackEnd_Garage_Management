using Garage_Management.Base.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Interface
{
    public interface IGenerateToken
    {
        /// <summary>
        /// Tạo JWT token từ thông tin user
        /// </summary>
        string GenerateJwtToken(User user);
        /// <summary>
        /// Tạo chuỗi refresh token
        /// </summary>
        /// <returns></returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Lấy tên authentication provider
        /// </summary>
        string GetAuthProvider();

        /// <summary>
        /// Lấy chuỗi token xác thực hiện tại
        /// </summary>
        string GetAuthToken();

        /// <summary>
        /// Thời gian hết hạn token (phút)
        /// </summary>
        int GetExpireToken();
    }
}
