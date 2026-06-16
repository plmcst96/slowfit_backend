using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/notification")]
[ApiController]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost("client-to-trainer")]
    public async Task<IActionResult> NotifyTrainerByClient([FromBody] ClientToTrainerNotificationRequest request)
    {
        if (request == null) return this.ApiBadRequest("invalid_notification", "I dati della notifica sono obbligatori.");
        if (!User.CanAccessUser(request.ClientId)) return this.ApiForbidden();
        return this.ToActionResult(await _notificationService.NotifyTrainerByClientAsync(request));
    }

    [HttpPost("trainer-to-client")]
    public async Task<IActionResult> NotifyClientByTrainer([FromBody] TrainerToClientNotificationRequest request)
    {
        if (request == null) return this.ApiBadRequest("invalid_notification", "I dati della notifica sono obbligatori.");
        if (!User.IsPersonalTrainer()) return this.ApiForbidden();
        return this.ToActionResult(await _notificationService.NotifyClientByTrainerAsync(request));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        if (!User.CanAccessUser(userId)) return this.ApiForbidden();
        return this.ToActionResult(await _notificationService.GetByUserAsync(userId));
    }

    [HttpDelete("{notificationId}")]
    public async Task<IActionResult> Delete(int notificationId)
    {
        var userId = User.GetUserId();
        return userId.HasValue
            ? this.ToActionResult(await _notificationService.DeleteAsync(notificationId, userId.Value))
            : this.ApiUnauthorized();
    }
}
