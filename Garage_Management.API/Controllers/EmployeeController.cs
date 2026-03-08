using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMechanicService _mechanicService;

        public EmployeeController(IMechanicService mechanicService)
        {
            _mechanicService = mechanicService;
        }

        [HttpGet("mechanics")]
        public async Task<IActionResult> GetMechanics()
        {
            var result = await _mechanicService.GetAllMechanicsAsync();
            return Ok(result);
        }
    }
}
