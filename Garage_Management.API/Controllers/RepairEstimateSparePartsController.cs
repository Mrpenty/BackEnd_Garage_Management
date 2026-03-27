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
        }
    }
}
