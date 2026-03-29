using Garage_Management.Application.DTOs.Notifications;
using Garage_Management.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage_Management.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

            public NotificationController(INotificationService notificationService)
            {
                _notificationService = notificationService;
            }

            [HttpPost("create")]
           
            public async Task<IActionResult> CreateNotification(
                [FromBody] CreateNotificationRequest request,
                CancellationToken ct = default)
            {
                var result = await _notificationService.CreateAndSendNotificationAsync(request, ct);
                return Ok(result);
            }
        }
}
