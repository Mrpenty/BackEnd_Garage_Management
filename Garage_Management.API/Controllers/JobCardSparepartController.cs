using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobCardSparepartController : ControllerBase
    {
        private readonly IJobCardSparePartService _service;

        public JobCardSparepartController(IJobCardSparePartService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<JobCardSparePartResponse>>>> GetAll(CancellationToken ct = default)
        {
            var data = await _service.GetAllAsync(ct);
            return Ok(ApiResponse<List<JobCardSparePartResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("job-cards/{jobCardId:int}")]
        public async Task<ActionResult<ApiResponse<List<JobCardSparePartResponse>>>> GetByJobCardId(
            int jobCardId,
            CancellationToken ct = default)
        {
            var data = await _service.GetByJobCardIdAsync(jobCardId, ct);
            return Ok(ApiResponse<List<JobCardSparePartResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpPost("{id}/spare-parts")]
        public async Task<ActionResult<ApiResponse<List<JobCardSparePartResponse>>>> AddSparePart(
            int id,
            [FromBody] AddMultipleSparePartsToJobCardDto dto,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.AddSparePartsAsync(id, dto, ct);

                if (data == null)
                    return NotFound(ApiResponse<List<JobCardSparePartResponse>>
                        .ErrorResponse("JobCard khong ton tai"));

                return Ok(ApiResponse<List<JobCardSparePartResponse>>
                    .SuccessResponse(data, "Them danh sach phu tung thanh cong"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<List<JobCardSparePartResponse>>
                    .ErrorResponse(ex.Message));
            }
        }

        [HttpDelete("{jobCardId:int}/spare-parts/{sparePartId:int}")]
        public async Task<ActionResult<ApiResponse<bool>>> RemoveSparePart(
            int jobCardId,
            int sparePartId,
            CancellationToken ct = default)
        {
            try
            {
                var result = await _service.RemoveSparePartAsync(jobCardId, sparePartId, ct);

                if (!result)
                    return NotFound(ApiResponse<bool>
                        .ErrorResponse("Khong tim thay phu tung"));

                return Ok(ApiResponse<bool>
                    .SuccessResponse(true, "Da huy phu tung"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<bool>
                    .ErrorResponse(ex.Message));
            }
        }

        [HttpPut("{jobCardId:int}/spare-parts/{sparePartId:int}")]
        public async Task<ActionResult<ApiResponse<JobCardSparePartResponse>>> UpdateSparePart(
            int jobCardId,
            int sparePartId,
            [FromBody] UpdateJobCardSparePartDto dto,
            CancellationToken ct = default)
        {
            try
            {
                var data = await _service.UpdateAsync(jobCardId, sparePartId, dto, ct);

                if (data == null)
                    return NotFound(ApiResponse<JobCardSparePartResponse>
                        .ErrorResponse("Không tìm thấy phụ tùng trong JobCard"));

                return Ok(ApiResponse<JobCardSparePartResponse>
                    .SuccessResponse(data, "Cập nhật phụ tùng thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<JobCardSparePartResponse>
                    .ErrorResponse(ex.Message));
            }
        }
    }
}
