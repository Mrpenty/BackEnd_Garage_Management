using Garage_Management.Application.DTOs.User;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Application.Interfaces.Services.Accounts
{
    public interface ICustomerService
    {
        /// <summary>
        /// Lấy danh sách khách hàng theo phân trang và lọc
        /// </summary>
        Task<ApiResponse<PagedResult<CustomerDto>>> GetPagedAsync(ParamQuery query, CancellationToken ct = default);

        /// <summary>
        /// Receptionist tạo khách hàng mới (tạo User + Customer)
        /// </summary>
        Task<ApiResponse<CustomerDto>> CreateCustomerByReceptionistAsync(CreateCustomerRequest request,CancellationToken ct = default);
    }
}
