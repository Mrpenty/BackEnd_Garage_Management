using Garage_Management.Application.DTOs.Workbays;

using Garage_Management.Application.Interfaces.Services;
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
            CancellationToken ct = default)
        {
            var result = await _service.GetListAsync(ct);
            return Ok(result);
        }
    }
}