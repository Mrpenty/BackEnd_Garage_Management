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

        /// <summary>
        /// Lấy danh sách phụ tùng theo hãng phụ tùng
        /// </summary>
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
