using Garage_Management.Application.DTOs.WarrantyServices;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarrantyServicesController : ControllerBase
    {
        private readonly IWarrantyServiceService _service;

        public WarrantyServicesController(IWarrantyServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<WarrantyServiceResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<WarrantyServiceResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<WarrantyServiceResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<WarrantyServiceResponse>.ErrorResponse("WarrantyService not found"));

            return Ok(ApiResponse<WarrantyServiceResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<WarrantyServiceResponse>>> Create(
            [FromBody] WarrantyServiceCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.WarrantyServiceId }, ApiResponse<WarrantyServiceResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<WarrantyServiceResponse>>> Update(
            int id,
            [FromBody] WarrantyServiceUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<WarrantyServiceResponse>.ErrorResponse("WarrantyService not found"));

            return Ok(ApiResponse<WarrantyServiceResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("WarrantyService not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
