using Garage_Management.Application.DTOs.JobCard;
using Garage_Management.Application.Services.JobCards;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
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

    }

}
