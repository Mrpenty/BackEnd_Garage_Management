using Garage_Management.Application.DTOs.RepairEstimates;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RepairEstimatesController : ControllerBase
    {
        private readonly IRepairEstimateService _service;

        public RepairEstimatesController(IRepairEstimateService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<RepairEstimateResponse>>>> GetAll(CancellationToken ct = default)
        {
            var data = await _service.GetAllAsync(ct);
            return Ok(ApiResponse<List<RepairEstimateResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<RepairEstimateDetailResponse>>> GetById(int id, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(id, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse("RepairEstimate not found"));

                return Ok(ApiResponse<RepairEstimateDetailResponse>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("job-cards/{jobCardId:int}")]
        public async Task<ActionResult<ApiResponse<List<RepairEstimateDetailResponse>>>> GetByJobCardId(
            int jobCardId,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByJobCardIdAsync(jobCardId, ct);
                if (data == null)
                    return NotFound(ApiResponse<List<RepairEstimateDetailResponse>>.ErrorResponse("JobCard not found"));

                return Ok(ApiResponse<List<RepairEstimateDetailResponse>>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<List<RepairEstimateDetailResponse>>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RepairEstimateDetailResponse>>> Create(
            [FromBody] RepairEstimateCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = data.RepairEstimateId },
                    ApiResponse<RepairEstimateDetailResponse>.SuccessResponse(data, "Created"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<RepairEstimateDetailResponse>>> UpdateStatus(
            int id,
            [FromBody] RepairEstimateStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(id, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse("RepairEstimate not found"));

                return Ok(ApiResponse<RepairEstimateDetailResponse>.SuccessResponse(data, "Updated status"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
        }
    }
}
