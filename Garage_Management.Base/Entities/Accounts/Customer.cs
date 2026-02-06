using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.JobCards;
using Garage_Management.Base.Entities.Vehiclies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Entities.Accounts
{
    /// <summary>
    /// Bảng Customer - Lưu thông tin của khách hàng trong gara
    /// </summary>
    public class Customer : AuditableEntity
    {
       
        public int CustomerId { get; set; }

       
        public string FirstName { get; set; } = string.Empty;

        
        public string LastName { get; set; } = string.Empty;

       

        /// <summary>
        /// Địa chỉ liên lạc của khách hàng
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Liên kết (tùy chọn) đến tài khoản hệ thống nếu khách hàng có tài khoản đăng nhập
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Tài khoản hệ thống gắn với khách hàng
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Danh sách xe thuộc sở hữu của khách hàng
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        /// <summary>
        /// Danh sách lịch hẹn dịch vụ của khách hàng
        /// </summary>
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        /// <summary>
        /// Danh sách phiếu sửa chữa (JobCard) của khách hàng
        /// </summary>
        public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();
    }
}
