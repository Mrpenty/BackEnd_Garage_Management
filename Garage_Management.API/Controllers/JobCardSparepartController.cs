using Garage_Management.Application.DTOs.JobCards;
using Garage_Management.Application.Interfaces.Services.JobCard;
using Garage_Management.Base.Entities.Services;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class JobCardSparepartController : ControllerBase
    {
        private readonly IJobCardSparePartService _service;

        public JobCardSparepartController(IJobCardSparePartService service)
        {
            _service = service;
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
