using Garage_Management.Application.DTOs.User;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Garage_Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Lấy danh sách khách hàng có phân trang và Lọc theo từ khóa (tên, sđt, email,biên số)
        /// Sắp xếp (theo CreatedAt giảm dần)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCustomers([FromQuery] ParamQuery query, CancellationToken ct)
        {
            try
            {
                var result = await _customerService.GetPagedAsync(query, ct);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Lỗi hệ thống"));
            }

        }
        /// <summary>
        /// Tạo khách hàng mới bởi nhân viên lễ tân
        /// </summary>
        [Authorize(Roles = "Receptionist")]
        [HttpPost("CreateByReceptionist")]
        public async Task<IActionResult> CreateCustomerByReceptionist([FromBody] CreateCustomerRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _customerService.CreateCustomerByReceptionistAsync(request, ct);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CustomerDto>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponse<CustomerDto>.ErrorResponse("Lỗi hệ thống"));
            }

        }
    }
}