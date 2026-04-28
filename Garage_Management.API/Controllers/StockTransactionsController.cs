using Garage_Management.Application.DTOs.Iventories.StockTransactions;
using Garage_Management.Application.Interfaces.Services.Inventories;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StockTransactionsController : ControllerBase
    {
        private readonly IStockTransactionService _service;

        public StockTransactionsController(IStockTransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<StockTransactionResponse>>>> GetPaged(
            [FromQuery] ParamQuery query,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(query, ct);
            return Ok(ApiResponse<PagedResult<StockTransactionResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<StockTransactionResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<StockTransactionResponse>.ErrorResponse("StockTransaction not found"));

            return Ok(ApiResponse<StockTransactionResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<StockTransactionResponse>>> Create(
            [FromBody] StockTransactionCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.StockTransactionId }, ApiResponse<StockTransactionResponse>.SuccessResponse(data, "Created"));
        }
    }
}
