using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.DTOResponse;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/user")]
[ApiController]
public class UserController(IUserService userService, INotificationService notificationService) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly INotificationService _notificationService = notificationService;

    [HttpGet]
    public async Task<IActionResult> GetUsersByRole([FromQuery] int roleId)
    {
        if (!User.IsPersonalTrainer()) return this.ApiForbidden();
        return this.ToActionResult(await _userService.GetUsersByRoleAsync(roleId));
    }

    [HttpGet("pt/{ptId}")]
    public async Task<IActionResult> GetUsersByPtId(int ptId)
    {
        if (!User.IsPersonalTrainer() && User.GetUserId() != ptId) return this.ApiForbidden();
        return this.ToActionResult(await _userService.GetUsersByPtIdAsync(ptId));
    }

    [HttpGet("alluser")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (!User.IsPersonalTrainer()) return this.ApiForbidden();
        return this.ToActionResult(await _userService.GetAllUsersAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProfile(int id)
    {
        if (!User.CanAccessUser(id)) return this.ApiForbidden();
        return this.ToActionResult(await _userService.GetProfileAsync(id));
    }

    [HttpGet("byEmail/{email}")]
    public async Task<IActionResult> GetProfileByEmail(string email)
    {
        var result = await _userService.GetProfileByEmailAsync(email);
        if (result.IsSuccess && result.Value != null && !User.CanAccessUser(result.Value.UserId)) return this.ApiForbidden();
        return this.ToActionResult(result);
    }

    [HttpPost("profile/{userId}")]
    public async Task<IActionResult> CreateProfile(int userId, [FromBody] AddProfile request)
    {
        if (!User.CanAccessUser(userId)) return this.ApiForbidden();
        return this.ToActionResult(await _userService.CreateProfileAsync(userId, request));
    }

    [HttpPost("update-fcm-token")]
    public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenRequest request)
    {
        if (request == null) return this.ApiBadRequest("invalid_fcm_token", "Token notifiche non valido.");
        if (!User.CanAccessUser(request.UserId)) return this.ApiForbidden();
        return this.ToActionResult(await _notificationService.UpdateFcmTokenAsync(request));
    }

    [HttpPut("profile/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserProfile request)
    {
        if (!User.CanAccessUser(id)) return this.ApiForbidden();
        return this.ToActionResult(await _userService.UpdateUserAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.IsPersonalTrainer()) return this.ApiForbidden();
        return this.ToActionResult(await _userService.DeleteAsync(id));
    }
}
