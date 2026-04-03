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
            var result = await _service.GetListAsync(status, ct);
            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<WorkBayDto>>> Create([FromBody] CreateWorkBayRequest request,
            CancellationToken ct = default)
        {
            var result = await _service.CreateWorkBayAsync(request, ct);
            return Ok(result);
        }
        [HttpPost("{id:int}/change-info")]

        public async Task<ActionResult<ApiResponse<WorkBayDto>>> ChangeStatus(int id, [FromBody] UpdateWorkBayRequest request,
            CancellationToken ct = default)
        {
            var result = await _service.UpdateWorkBayAsync(id, request, ct);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);

        }

    }
}
