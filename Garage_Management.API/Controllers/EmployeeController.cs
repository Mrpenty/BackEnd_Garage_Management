using Garage_Management.Application.DTOs.Employee;
using Garage_Management.Application.Interfaces.Repositories;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Application.Interfaces.Services.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IMechanicService _mechanicService;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IMechanicService mechanicService, IEmployeeService employeeService)
        {
            _mechanicService = mechanicService;
            _employeeService = employeeService;
        }

        [HttpGet("mechanics")]
        public async Task<IActionResult> GetMechanics()
        {
            var result = await _mechanicService.GetAllMechanicsAsync();
            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest request)
        {
            var result = await _employeeService.CreateEmployeeAsync(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
