using Garage_Management.Application.DTOs.Appointments;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
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

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<AppointmentResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<AppointmentResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<AppointmentResponse>.ErrorResponse("Appointment not found"));

            return Ok(ApiResponse<AppointmentResponse>.SuccessResponse(data, "OK"));
        }

        [HttpGet("by-customer/{customerId:int}")]
        public async Task<ActionResult<ApiResponse<PagedResult<AppointmentResponse>>>> GetByCustomer(
            int customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetByCustomerIdAsync(page, pageSize, customerId, ct);
            return Ok(ApiResponse<PagedResult<AppointmentResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<AppointmentResponse>>> Create(
            [FromBody] AppointmentCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.AppointmentId }, ApiResponse<AppointmentResponse>.SuccessResponse(data, "Created"));
        }

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
