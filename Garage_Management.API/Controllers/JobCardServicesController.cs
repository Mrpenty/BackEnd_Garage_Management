using Garage_Management.Application.DTOs.JobCardServices;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardServicesController : ControllerBase
    {
        private readonly IJobCardServiceService _service;

        public JobCardServicesController(IJobCardServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<JobCardServiceResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<JobCardServiceResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<JobCardServiceResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<JobCardServiceResponse>.ErrorResponse("JobCardService not found"));

            return Ok(ApiResponse<JobCardServiceResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<JobCardServiceResponse>>> Create(
            [FromBody] JobCardServiceCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);

                if (!data.Success)
                {
                    return BadRequest(data);
                }
                return CreatedAtAction(nameof(GetById), new { id = data.Data.JobCardServiceId }, data);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<JobCardServiceResponse>
                {
                    Success = false,
                    Message = $"Lỗi Controller: {ex.Message} | Inner: {ex.InnerException?.Message}"
                });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<JobCardServiceResponse>>> Update(
            int id,
            [FromBody] JobCardServiceUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<JobCardServiceResponse>.ErrorResponse("JobCardService not found"));

            return Ok(ApiResponse<JobCardServiceResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpPatch("service/{serviceId:int}/status")]
        public async Task<ActionResult<ApiResponse<JobCardServiceResponse>>> UpdateStatusByServiceId(
            int serviceId,
            [FromBody] JobCardServiceStatusUpdateRequest request,
            [FromQuery] int? jobCardId = null,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusByServiceIdAsync(serviceId, jobCardId, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<JobCardServiceResponse>.ErrorResponse("JobCardService not found"));

                return Ok(ApiResponse<JobCardServiceResponse>.SuccessResponse(data, "Updated status successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<JobCardServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("JobCardService not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
