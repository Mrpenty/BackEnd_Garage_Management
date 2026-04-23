using Garage_Management.Base.Common.Base;
using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Entities.Inventories;
using Garage_Management.Base.Entities.JobCards;
using System.Collections.Generic;

namespace Garage_Management.Base.Entities.Branches
{
    /// <summary>
    /// Bảng Branch - Chi nhánh gara trong chuỗi
    /// </summary>
    public class Branch : AuditableEntity
    {
        public int BranchId { get; set; }

        /// <summary>
        /// Mã chi nhánh (VD: HN-01, SG-02)
        /// </summary>
        public string BranchCode { get; set; } = string.Empty;

        /// <summary>
        /// Tên chi nhánh
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Địa chỉ chi nhánh
        /// </summary>
        public string Address { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        /// <summary>
        /// Nhân viên được chỉ định làm quản lý chi nhánh (tùy chọn)
        /// </summary>
        public int? ManagerEmployeeId { get; set; }
        public Employee? ManagerEmployee { get; set; }

        /// <summary>
        /// Chi nhánh đang hoạt động hay đã tạm ngưng
        /// </summary>
        public bool IsActive { get; set; } = true;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<WorkBay> WorkBays { get; set; } = new List<WorkBay>();
        public ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();
    }
}
