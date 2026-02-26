using Garage_Management.Application.DTOs.ServiceWarrantyPolicies;
using Garage_Management.Application.Interfaces.Services;
using Garage_Management.Base.Common.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceWarrantyPoliciesController : ControllerBase
    {
        private readonly IServiceWarrantyPolicyService _service;

        public ServiceWarrantyPoliciesController(IServiceWarrantyPolicyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<ServiceWarrantyPolicyResponse>>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default)
        {
            var data = await _service.GetPagedAsync(page, pageSize, ct);
            return Ok(ApiResponse<PagedResult<ServiceWarrantyPolicyResponse>>.SuccessResponse(data, "OK"));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<ServiceWarrantyPolicyResponse>>> GetById(int id, CancellationToken ct = default)
        {
            var data = await _service.GetByIdAsync(id, ct);
            if (data == null)
                return NotFound(ApiResponse<ServiceWarrantyPolicyResponse>.ErrorResponse("ServiceWarrantyPolicy not found"));

            return Ok(ApiResponse<ServiceWarrantyPolicyResponse>.SuccessResponse(data, "OK"));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ServiceWarrantyPolicyResponse>>> Create(
            [FromBody] ServiceWarrantyPolicyCreateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = data.PolicyId }, ApiResponse<ServiceWarrantyPolicyResponse>.SuccessResponse(data, "Created"));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse<ServiceWarrantyPolicyResponse>>> Update(
            int id,
            [FromBody] ServiceWarrantyPolicyUpdateRequest request,
            CancellationToken ct = default)
        {
            var data = await _service.UpdateAsync(id, request, ct);
            if (data == null)
                return NotFound(ApiResponse<ServiceWarrantyPolicyResponse>.ErrorResponse("ServiceWarrantyPolicy not found"));

            return Ok(ApiResponse<ServiceWarrantyPolicyResponse>.SuccessResponse(data, "Updated"));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct = default)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok)
                return NotFound(ApiResponse<object>.ErrorResponse("ServiceWarrantyPolicy not found"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Deleted"));
        }
    }
}
