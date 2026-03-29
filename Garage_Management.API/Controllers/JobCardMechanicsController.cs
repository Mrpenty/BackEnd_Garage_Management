using Garage_Management.Application.DTOs.JobCardMechanics;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Garage_Management.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobCardMechanicsController : ControllerBase
    {
        private readonly IJobCardMechanicService _service;

        public JobCardMechanicsController(IJobCardMechanicService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lấy danh sách phiếu sửa chữa được phân công cho thợ máy theo EmployeeId
        /// </summary>
        [HttpGet("ForMechanic")]
        public async Task<IActionResult> GetByEmployee([FromQuery] ParamQuery param, CancellationToken ct)
        {
            var response = await _service.GetJobCardsByEmployeeAsync(param, ct);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);

        }
        /// <summary>
        /// Cập nhật trạng thái công việc của thợ máy trong phiếu sửa chữa
        /// </summary>
        [HttpPatch("{jobCardId}/mechanic/status")]
        public async Task<IActionResult> UpdateMechanicStatus(int jobCardId, [FromBody] UpdateJobCardMechanicStatusDto dto)
        {
            var response = await _service.UpdateStatusAsync(jobCardId, dto);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
