using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.JobCards;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class JobCardSparepartController : ControllerBase
{
    private readonly IJobCardSparePartService _service;

    public JobCardSparepartController(IJobCardSparePartService service)
    {
        _service = service;
    }

    [HttpPost("{id}/spare-parts")]
    public async Task<ActionResult<ApiResponse<JobCardSparePart>>> AddSparePart(
        int id,
        [FromBody] AddSparePartToJobCardDto dto,
        CancellationToken ct = default)
    {
        try
        {
            var data = await _service.AddSparePartAsync(id, dto, ct);

            if (data == null)
                return NotFound(ApiResponse<JobCardSparePart>
                    .ErrorResponse("JobCard không tồn tại"));

            return Ok(ApiResponse<JobCardSparePart>
                .SuccessResponse(data, "Thêm phụ tùng thành công"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<JobCardSparePart>
                .ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("spare-parts/{jobCardSparePartId:int}")]
    public async Task<ActionResult<ApiResponse<bool>>> RemoveSparePart(
        int jobCardSparePartId,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _service.RemoveSparePartAsync(jobCardSparePartId, ct);

            if (!result)
                return NotFound(ApiResponse<bool>
                    .ErrorResponse("Không tìm thấy phụ tùng"));

            return Ok(ApiResponse<bool>
                .SuccessResponse(true, "Đã hủy phụ tùng"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>
                .ErrorResponse(ex.Message));
        }
    }
}