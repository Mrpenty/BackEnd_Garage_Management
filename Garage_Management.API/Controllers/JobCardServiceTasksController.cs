using Garage_Management.Application.DTOs.JobCardServiceTasks;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardServiceTasksController : ControllerBase
    {
        private readonly IJobCardServiceTaskService _service;

        public JobCardServiceTasksController(IJobCardServiceTaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<JobCardServiceTaskResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<JobCardServiceTaskResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<JobCardServiceTaskResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<JobCardServiceTaskResponse>.ErrorResponse("JobCardServiceTask not found"));

            return Ok(ApiResponse<JobCardServiceTaskResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<JobCardServiceTaskResponse>>> Create(
            [FromBody] JobCardServiceTaskCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.JobCardServiceTaskId }, ApiResponse<JobCardServiceTaskResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<JobCardServiceTaskResponse>>> Update(
            int id,
            [FromBody] JobCardServiceTaskUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<JobCardServiceTaskResponse>.ErrorResponse("JobCardServiceTask not found"));

            return Ok(ApiResponse<JobCardServiceTaskResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("JobCardServiceTask not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
