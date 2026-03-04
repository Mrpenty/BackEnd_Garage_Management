using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var data = await _service.GetPagedAsync(query, ct);
            return Ok(ApiResponse<PagedResult<AppointmentResponse>>.SuccessResponse(data, "OK"));
        }
        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Lấy chi tiết lịch đặt được phân trang
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Appointment not found"));

            return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "OK"));
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
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.AppointmentId }, ApiResponse<AppointmentResponse>.SuccessResponse(data, "Created"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Cập nhật lịch đặt
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> Update(
            int id,
            [FromBody] AppointmentUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Appointment not found"));

            return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "Updated"));
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
            var data = await _service.UpdateStatusAsync(id, request.Status, ct);
            if (data == null)
                return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Appointment not found"));

            return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "Updated"));
        }

        ///Author: KhanhDV
        ///Created Date: 13-2-2026
        /// <summary>
        /// Xóa lịch đặt được phân trang
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Appointment not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
