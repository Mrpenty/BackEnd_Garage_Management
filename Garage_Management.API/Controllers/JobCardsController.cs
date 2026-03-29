using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Common.Models;
using Garage_Management.Base.Entities.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardsController : ControllerBase
    {
        private readonly IJobCardService _service;

        public JobCardsController(IJobCardService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateJobCardDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var userId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)!.Value
                );

                var result = await _service.CreateAsync(dto, userId, cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }


        [HttpGet("active")]
        public async Task<IActionResult> GetActive(
     [FromQuery] string? search,
     [FromQuery] string? sortBy,
     [FromQuery] string? sortDirection,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetActiveAsync(
                search,
                sortBy,
                sortDirection,
                page,
                pageSize);

            return Ok(result);
        }


        [HttpGet("{id}")]
            public async Task<IActionResult> GetById(int id)
            {
                var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateJobCardStatusDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status, cancellationToken);

            if (!result) return NotFound();

            return NoContent();
        }
        [HttpPost("{id}/assign-mechanic")]
        public async Task<IActionResult> AssignMechanic( int id,AssignMechanicDto dto,CancellationToken cancellationToken)
        {
            var result = await _service.AssignMechanicAsync(id, dto, cancellationToken);

            if (!result) return NotFound();

            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateJobCardDto dto,CancellationToken cancellationToken)
        {
            var result = await _service.UpdateAsync(id, dto, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpPost("{id}/services")]
        public async Task<IActionResult> AddService( int id, AddServiceToJobCardDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.AddServiceAsync(id, dto, cancellationToken);

            if (!result) return BadRequest();

            return NoContent();
        }

        
        [HttpPost("assign-workbay")]
        public async Task<IActionResult> AssignWorkBay(
            [FromBody] AssignWorkBayRequestDto dto,
             CancellationToken cancellationToken)
        {
            var result = await _service.AssignWorkBayAsync(dto, cancellationToken);

            if (!result)
                return BadRequest("Cannot assign work bay");

            return Ok("Work bay assigned successfully");
        }


        [HttpPost("release-workbay")]
        public async Task<IActionResult> ReleaseWorkBay(
    [FromBody] ReleaseWorkBayDto dto,
    CancellationToken cancellationToken)
        {
            var result = await _service.ReleaseWorkBayAsync(dto, cancellationToken);

            if (!result)
                return BadRequest("Cannot release work bay");

            return Ok("Work bay released successfully");
        }

        [HttpGet("supervisor/{supervisorId}")]
        public async Task<IActionResult> GetBySupervisorId(int supervisorId)
        {
            var jobCards = await _service.GetJobCardsBySupervisorIdAsync(supervisorId);
            return Ok(jobCards);
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var jobCards = await _service.GetJobCardsByCustomerIdAsync(customerId);
            return Ok(jobCards);
        }

        [HttpPatch("{id}/progress-update")]
        [Authorize(Roles = "Mechanic,Supervisor")]
        public async Task<IActionResult> UpdateRepairProgress(int id, UpdateJobCardProgressDto dto, CancellationToken cancellationToken)
        {
            try
            {

                var result = await _service.UpdateRepairProgressAsync(id, dto, cancellationToken);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/progress-viewdetail")]
        [Authorize]
        public async Task<IActionResult> ViewRepairProgress(int id, CancellationToken cancellationToken)
        {
            try
            { 
                var result = await _service.ViewRepairProgressAsync(id,  cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
