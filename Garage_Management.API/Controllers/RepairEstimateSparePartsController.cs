using Garage_Management.Application.DTOs.RepairEstimateSpareParts;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepairEstimateSparePartsController : ControllerBase
    {
        private readonly IRepairEstimateSparePartService _service;

        public RepairEstimateSparePartsController(IRepairEstimateSparePartService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<RepairEstimateSparePartResponse>>> Create(
            [FromBody] RepairEstimateSparePartCreateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.CreateAsync(request, ct);
                return Ok(ApiResponse<RepairEstimateSparePartResponse>.SuccessResponse(data, "Created"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateSparePartResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<RepairEstimateSparePartResponse>.ErrorResponse(ex.Message));
            }
        }

        [HttpPatch("{repairEstimateId:int}/{sparePartId:int}/status")]
        public async Task<ActionResult<ApiResponse<RepairEstimateSparePartResponse>>> UpdateStatus(
            int repairEstimateId,
            int sparePartId,
            [FromBody] RepairEstimateSparePartStatusUpdateRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateStatusAsync(repairEstimateId, sparePartId, request, ct);
                if (data == null)
                    return NotFound(ApiResponse<RepairEstimateSparePartResponse>.ErrorResponse("RepairEstimateSparePart not found"));

                return Ok(ApiResponse<RepairEstimateSparePartResponse>.SuccessResponse(data, "Updated status"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<RepairEstimateSparePartResponse>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponse<RepairEstimateSparePartResponse>.ErrorResponse(ex.Message));
            }
        }
    }
}
