using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service)
        {
            _service = service;
        }
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt có phân trang, lọc, tìm kiếm, sắp xếp
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AppointmentResponse>>>> GetPaged(
            [FromQuery] AppointmentQuery query,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetPagedAsync(query, ct);
                return Ok(ApiResponse<PagedResult<AppointmentResponse>>.SuccessResponse(data, "Lấy danh sách đặt lịch thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<PagedResult<AppointmentResponse>>.ErrorResponse(ex.Message));
            }
        }
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết lịch đặt được phân trang
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Không tìm thấy lịch đặt"));
                return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "Lấy chi tiết lịch đặt thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<AppointmentResponse>.ErrorResponse(ex.Message));
            }
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy danh sách lịch đặt theo khách hàng được phân trang
        /// </summary>
        [HttpGet("Customer/MyAppointment")]
        public async Task<ActionResult<ApiResponse<PagedResult<AppointmentResponse>>>> GetByCustomer(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var result = await _service.GetMyAppointmentsAsync(page, pageSize, ct);

            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Tạo mới lịch đặt
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> Create(
            [FromBody] AppointmentCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = data.AppointmentId }, ApiResponse<AppointmentResponse>.SuccessResponse(data, "Tạo lịch đặt thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<AppointmentResponse>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Duyệt / đổi trạng thái lịch đặt
        /// </summary>
        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> UpdateStatus(
            int id,
            [FromBody] AppointmentStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(id, request.Status, ct);
                if (data == null)
                    return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Không tìm thấy lịch đặt"));

                return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "Cập nhật lịch đặt thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<AppointmentResponse>.ErrorResponse(ex.Message));
            }
        }

    }
}
