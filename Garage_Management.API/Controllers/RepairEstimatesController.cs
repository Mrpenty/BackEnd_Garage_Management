using Garage_Management.Application.DTOs.RepairEstimates;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse("RepairEstimate not found"));

            return Ok(ApiResponse<RepairEstimateDetailResponse>.SuccessResponse(data, "OK"));
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateDetailResponse>.ErrorResponse(ex.Message));
            }
        }
    }
}
