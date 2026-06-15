using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/login")]
[ApiController]
[AllowAnonymous]
public class UserLoginController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        var result = await _authService.LoginAsync(userLogin);
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();
        return this.ToActionResult(await _authService.RefreshAsync(userId.Value));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        if (userId == null) return Unauthorized();
        return this.ToActionResult(await _authService.GetMeAsync(userId.Value));
    }
}
