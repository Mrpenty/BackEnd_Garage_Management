using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Base.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services.Accounts
{
    public interface IEmployeeService
    {
        /// <summary>
        /// Tạo tài khoản nhân viên mới
        /// </summary>
        Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken ct = default);
    }
}
