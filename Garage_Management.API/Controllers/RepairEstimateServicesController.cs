using Garage_Management.Application.DTOs.RepairEstimateServices;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepairEstimateServicesController : ControllerBase
    {
        private readonly IRepairEstimateServiceService _service;

        public RepairEstimateServicesController(IRepairEstimateServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<RepairEstimateServiceResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetPagedAsync(page, pageSize, ct);
                return Ok(ApiResponse<PagedResult<RepairEstimateServiceResponse>>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PagedResult<RepairEstimateServiceResponse>>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("{repairEstimateId:int}/{serviceId:int}")]
        public async Task<ActionResult<ApiResponse<RepairEstimateServiceResponse>>> GetById(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            try
            {
                var data = await _service.GetByIdAsync(repairEstimateId, serviceId, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse("RepairEstimateService not found"));

                return Ok(ApiResponse<RepairEstimateServiceResponse>.SuccessResponse(data, "OK"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RepairEstimateServiceResponse>>> Create(
            [FromBody] RepairEstimateServiceCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return CreatedAtAction(nameof(GetById), new { repairEstimateId = data.RepairEstimateId, serviceId = data.ServiceId }, ApiResponse<RepairEstimateServiceResponse>.SuccessResponse(data, "Created"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{repairEstimateId:int}/{serviceId:int}")]
        public async Task<ActionResult<ApiResponse<RepairEstimateServiceResponse>>> Update(
            int repairEstimateId,
            int serviceId,
            [FromBody] RepairEstimateServiceUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateAsync(repairEstimateId, serviceId, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse("RepairEstimateService not found"));

                return Ok(ApiResponse<RepairEstimateServiceResponse>.SuccessResponse(data, "Updated"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{repairEstimateId:int}/{serviceId:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int repairEstimateId, int serviceId, CancellationToken ct = default)
        {
            try
            {
                var ok = await _service.DeleteAsync(repairEstimateId, serviceId, ct);
                if (!ok)
                    return NotFound(ApiResponse<object>.ErrorResponse("RepairEstimateService not found"));

                return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }

        [HttpPatch("{repairEstimateId:int}/{serviceId:int}/status")]
        public async Task<ActionResult<ApiResponse<RepairEstimateServiceResponse>>> UpdateStatus(
            int repairEstimateId,
            int serviceId,
            [FromBody] RepairEstimateServiceStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(repairEstimateId, serviceId, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse("RepairEstimateService not found"));

                return Ok(ApiResponse<RepairEstimateServiceResponse>.SuccessResponse(data, "Updated status"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<RepairEstimateServiceResponse>.ErrorResponse(ex.Message));
            }
        }
    }
}
