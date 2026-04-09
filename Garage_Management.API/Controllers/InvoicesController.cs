using Garage_Management.Application.DTOs.Invoices;
using Garage_Management.Application.Interfaces.Services.Invoices;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Common.Models.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoicesController(IInvoiceService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lay danh sach hoa don co phan trang, loc, tim kiem
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResponse>>>> GetPaged(
            [FromQuery] InvoiceQuery query,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, ct);
            return Ok(ApiResponse<PagedResult<InvoiceResponse>>.SuccessResponse(data, "OK"));
        }

        /// <summary>
        /// Lay chi tiet hoa don theo ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<InvoiceResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<InvoiceResponse>.ErrorResponse("Invoice not found"));

            return Ok(ApiResponse<InvoiceResponse>.SuccessResponse(data, "OK"));
        }

        /// <summary>
        /// Tao hoa don moi tu JobCard
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Create(
            [FromBody] InvoiceCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.InvoiceId },
                ApiResponse<InvoiceResponse>.SuccessResponse(data, "Created"));
        }

        /// <summary>
        /// Cap nhat hoa don
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<InvoiceResponse>>> Update(
            int id,
            [FromBody] InvoiceUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<InvoiceResponse>.ErrorResponse("Invoice not found"));

            return Ok(ApiResponse<InvoiceResponse>.SuccessResponse(data, "Updated"));
        }

        /// <summary>
        /// Xoa hoa don
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("Invoice not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
