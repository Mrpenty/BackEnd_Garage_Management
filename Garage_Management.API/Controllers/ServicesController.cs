using Garage_Management.Application.DTOs.Services;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private const string ManagerRoles = "Supervisor,Accountant";
        private const string AccountantRoles = "Accountant";

        private readonly IServiceService _service;

        public ServicesController(IServiceService service)
        {
            _service = service;
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách dịch vụ được phân trang
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? hasPrice = null,
            [FromQuery] int? vehicleTypeId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = true,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetPagedAsync(page, pageSize, keyword, isActive, hasPrice, vehicleTypeId, minPrice, maxPrice, sortBy, sortDesc, ct);
                return Ok(ApiResponse<PagedResult<ServiceResponse>>.SuccessResponse(data, "Lấy danh sách dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<PagedResult<ServiceResponse>>.ErrorResponse(ex.Message));
            }

        }

        ///Author: KhanhDV
        ///Created Date: 6-3-2026
        /// <summary>
        /// Lấy danh sách dịch vụ theo loại xe (VehicleType)
        /// </summary>
        [HttpGet("by-vehicle-type/{vehicleTypeId:int}")]
        public async Task<ActionResult<ApiResponse<List<ServiceResponse>>>> GetByVehicleType(
            int vehicleTypeId,
            CancellationToken ct = default)
        {
            var data = await _service.GetByVehicleTypeAsync(vehicleTypeId, ct);
            return Ok(ApiResponse<List<ServiceResponse>>.SuccessResponse(data, "Lấy danh sách dịch vụ theo loại xe thành công"));
        }

        [HttpGet("service-vehicle-type-pairs")]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceVehicleTypePairResponse>>>> GetServiceVehicleTypePairs(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            try
            {
                var result = await _service.GetServiceVehicleTypePairsAsync(page, pageSize, ct);
                return Ok(ApiResponse<PagedResult<ServiceVehicleTypePairResponse>>.SuccessResponse(result, "Lấy danh sách cặp dịch vụ - loại xe thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<PagedResult<ServiceVehicleTypePairResponse>>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết 1 dịch vụ
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Không tìm thấy dịch vụ"));
                return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "Lấy dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo 1 dịch vụ mới (chưa có giá, mặc định IsActive=false)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> Create(
            [FromBody] ServiceCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = data.ServiceId }, ApiResponse<ServiceResponse>.SuccessResponse(data, "Tạo dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Kế toán cập nhật giá dịch vụ.
        /// </summary>
        [HttpPatch("{id:int}/price")]
        [Authorize(Roles = AccountantRoles)]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> UpdatePrice(
            int id,
            [FromBody] ServicePriceUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdatePriceAsync(id, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Không tìm thấy dịch vụ"));
                return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "Cập nhật giá dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceResponse>.ErrorResponse(ex.Message));
            }
            
        }

        /// <summary>
        /// Quản lý bật/tắt dịch vụ qua IsActive.
        /// </summary>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> UpdateStatus(
            int id,
            [FromBody] ServiceUpdateStatusRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(id, request.IsActive, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Không tìm thấy dịch vụ"));
                return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "Cập nhật trạng thái dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deactivate 1 service.
        /// </summary>
        [HttpPatch("{id:int}/deactivate")]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<ServiceResponse>>> Deactivate(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.DeactivateAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceResponse>.ErrorResponse("Không tìm thấy dịch vụ"));
                return Ok(ApiResponse<ServiceResponse>.SuccessResponse(data, "Ngừng kích hoạt dịch vụ thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceResponse>.ErrorResponse(ex.Message));
            }
        }
    }
}
