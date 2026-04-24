using Garage_Management.Application.DTOs.ServiceTasks;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTasksController : ControllerBase
    {
        private const string ManagerRoles = "Supervisor,Accountant";

        private readonly IServiceTaskService _service;

        public ServiceTasksController(IServiceTaskService service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceTaskResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<ServiceTaskResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ServiceTaskResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceTaskResponse>.ErrorResponse("t�c v? d?ch v? kh�ng t�m th?y"));
                return Ok(ApiResponse<ServiceTaskResponse>.SuccessResponse(data, "L?y t�c v? d?ch v? th�nh c�ng"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceTaskResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("by-service/{serviceId:int}")]
        public async Task<ActionResult<ApiResponse<List<ServiceTaskResponse>>>> GetByServiceId(int serviceId, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByServiceIdAsync(serviceId, ct);
                return Ok(ApiResponse<List<ServiceTaskResponse>>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<List<ServiceTaskResponse>>.ErrorResponse(ex.Message));
            }   
        }

        [HttpPost]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<ServiceTaskResponse>>> Create(
            [FromBody] ServiceTaskCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { id = data.ServiceTaskId }, ApiResponse<ServiceTaskResponse>.SuccessResponse(data, "T?o t�c v? d?ch v? th�nh c�ng"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceTaskResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = ManagerRoles)]
        public async Task<ActionResult<ApiResponse<ServiceTaskResponse>>> Update(
            int id,
            [FromBody] ServiceTaskUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateAsync(id, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<ServiceTaskResponse>.ErrorResponse("T�c v? d?ch v? kh�ng t�m th?y"));

                return Ok(ApiResponse<ServiceTaskResponse>.SuccessResponse(data, "C?p nh?t t�c v? d?ch v? th�nh c�ng"));
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ApiResponse<ServiceTaskResponse>.ErrorResponse(ex.Message));
            }
        }

    }
}
