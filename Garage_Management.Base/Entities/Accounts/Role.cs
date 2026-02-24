using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng Role - Lưu thông tin vai trò
    /// </summary>
    public class Role
    {
        public int RoleId { get; set; }

        /// <summary>
        /// Tên vai trò 
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách tài khoản thuộc vai trò này
        /// </summary>
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}


