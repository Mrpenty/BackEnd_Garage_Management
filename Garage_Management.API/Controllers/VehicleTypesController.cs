using Garage_Management.Application.DTOs.Vehicles.VehicleType;
using Garage_Management.Application.Interfaces.Services.Vehiclies;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleTypesController : ControllerBase
    {
        private readonly IVehicleTypeService _service;

        public VehicleTypesController(IVehicleTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<VehicleTypeResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<VehicleTypeResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<VehicleTypeResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleTypeResponse>.ErrorResponse("VehicleType not found"));

            return Ok(ApiResponse<VehicleTypeResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehicleTypeResponse>>> Create(
            [FromBody] VehicleTypeCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.VehicleTypeId }, ApiResponse<VehicleTypeResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPatch("{id:int}/deactivate")]
        public async Task<ActionResult<ApiResponse<VehicleTypeResponse>>> Deactivate(int id, CancellationToken ct = default)
        {
            var data = await _service.DeactivateAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleTypeResponse>.ErrorResponse("VehicleType not found"));

            return Ok(ApiResponse<VehicleTypeResponse>.SuccessResponse(data, "Deactivated"));
        }

        [HttpPatch("{id:int}/activate")]
        public async Task<ActionResult<ApiResponse<VehicleTypeResponse>>> Activate(int id, CancellationToken ct = default)
        {
            var data = await _service.ActivateAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<VehicleTypeResponse>.ErrorResponse("VehicleType not found"));

            return Ok(ApiResponse<VehicleTypeResponse>.SuccessResponse(data, "Activated"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("VehicleType not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
