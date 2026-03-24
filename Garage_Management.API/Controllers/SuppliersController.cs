using Garage_Management.Application.DTOs.Inventories.Suppliers;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SuppliersController(ISupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<SupplierResponse>>>> GetPaged([FromQuery] ParamQuery query, CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, ct);
            return Ok(ApiResponse<PagedResult<SupplierResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SupplierResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<SupplierResponse>.ErrorResponse("Supplier not found"));

            return Ok(ApiResponse<SupplierResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SupplierResponse>>> Create([FromBody] SupplierCreateRequest request, CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.SupplierId }, ApiResponse<SupplierResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<SupplierResponse>>> Update(int id, [FromBody] SupplierUpdateRequest request, CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<SupplierResponse>.ErrorResponse("Supplier not found"));

            return Ok(ApiResponse<SupplierResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<ApiResponse<SupplierResponse>>> UpdateStatus(int id, [FromBody] SupplierUpdateStatusRequest request, CancellationToken ct = default)
        {
            var data = await _service.UpdateStatusAsync(id, request.IsActive, ct);
            if (data == null)
                return NotFound(ApiResponse<SupplierResponse>.ErrorResponse("Supplier not found"));

            return Ok(ApiResponse<SupplierResponse>.SuccessResponse(data, "Updated status"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Supplier not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
