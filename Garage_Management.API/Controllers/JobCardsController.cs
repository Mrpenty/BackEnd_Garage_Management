using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create(CreateJobCardDto dto, CancellationToken cancellationToken)
        {
            var result = await _service.CreateAsync(dto, cancellationToken);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            return Ok(await _service.GetActiveAsync());
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

    }

}
