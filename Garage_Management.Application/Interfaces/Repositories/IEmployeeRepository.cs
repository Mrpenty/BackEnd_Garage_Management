using Garage_Management.Base.Entities.Accounts;
using Garage_Management.Base.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        Task<Employee?> GetByUserIdAsync(int userId);
        
        /// <summary>
        /// Lấy nhân viên có role Identity "Mechanic", lọc tuỳ chọn theo chi nhánh.
        /// </summary>
        Task<List<Employee>> GetAllMechanicsAsync(int? branchId = null, CancellationToken ct = default);
    }
}
