using Garage_Management.Application.DTOs.Workbays;
using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Enums;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkBaysController : ControllerBase
    {
        private readonly IWorkBayService _service;

        public WorkBaysController(IWorkBayService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách WorkBay
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<WorkBayDto>>>> GetList(
            [FromQuery] WorkBayStatus? status,
            CancellationToken ct = default)
        {
            var data = await _service.GetListAsync(status, ct);

            return Ok(new ApiResponse<IEnumerable<WorkBayDto>>
            {
                Success = true,
                Data = data
            });
        }

        /// <summary>
        /// Lấy danh sách JobCard thuộc một WorkBay (bao gồm queue)
        /// </summary>
        [HttpGet("{id}/jobcards")]
        public async Task<ActionResult<ApiResponse<List<JobCardListDto>>>> GetJobCardsByWorkBay(
            int id,
            CancellationToken cancellationToken)
        {
            var data = await _service.GetJobCardsByWorkBayAsync(id, cancellationToken);

            if (data == null)
            {
                return NotFound(new ApiResponse<List<JobCardListDto>>
                {
                    Success = false,
                    Message = "WorkBay not found"
                });
            }

            return Ok(new ApiResponse<List<JobCardListDto>>
            {
                Success = true,
                Data = data
            });
        }
    }
}