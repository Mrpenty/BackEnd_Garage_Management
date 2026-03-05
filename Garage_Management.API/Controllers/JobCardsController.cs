using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Repositories.Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services;
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
        public async Task<IActionResult> Create(CreateJobCardDto dto,CancellationToken cancellationToken)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var result = await _service.CreateAsync(dto, userId, cancellationToken);
            if (result == null)
            {
                return Conflict("Vehicle already has an active job card.");
            }

            return Ok(result);
        }


        [HttpGet("active")]
        public async Task<IActionResult> GetActive(
     string? search,
     string? from,
     string? to)
        {
            return Ok(await _service.GetActiveAsync(search, from, to));
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

        [HttpPost("{id}/spare-parts")]
        public async Task<IActionResult> AddSparePart(int id, AddSparePartToJobCardDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.AddSparePartAsync(id, dto, cancellationToken);

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



    }

}
