using Garage_Management.Application.DTOs.Iventories;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoriesController(IInventoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<InventoryResponse>>>> GetPaged(
            [FromQuery] ParamQuery query,
            CancellationToken ct = default)
        {
            var result = await _service.GetPagedAsync(query, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<InventoryResponse>.ErrorResponse("Inventory not found"));

            return Ok(ApiResponse<InventoryResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> Create(
            [FromBody] InventoryCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.SparePartId }, ApiResponse<InventoryResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> Update(
            int id,
            [FromBody] InventoryUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<InventoryResponse>.ErrorResponse("Inventory not found"));

            return Ok(ApiResponse<InventoryResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<InventoryResponse>>> UpdateStatus(
            int id,
            [FromBody] InventoryUpdateStatusRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateStatusAsync(id, request.IsActive, ct);
            if (data == null)
                return NotFound(ApiResponse<InventoryResponse>.ErrorResponse("Inventory not found"));

            return Ok(ApiResponse<InventoryResponse>.SuccessResponse(data, "Updated status"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Inventory not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }

        [HttpGet("by-brand/{brandId:int}")]
        public async Task<ActionResult<ApiResponse<List<InventoryResponse>>>> GetByBrand(
            int brandId,
            CancellationToken ct = default)
        {
            var data = await _service.GetByBrandIdAsync(brandId, ct);
            return Ok(ApiResponse<List<InventoryResponse>>.SuccessResponse(data, "OK"));
        }
    }
}
