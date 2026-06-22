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

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        return this.ToActionResult(await _authService.RefreshAsync(request));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        return this.ToActionResult(await _authService.LogoutAsync(request));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.GetUserId();
        if (userId == null) return this.ApiUnauthorized();
        return this.ToActionResult(await _authService.GetMeAsync(userId.Value, User.GetRoleId()));
    }
}
