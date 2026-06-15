using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/register")]
[ApiController]
[AllowAnonymous]
public class UserRegisterController(IUserRegistrationService userRegistrationService) : ControllerBase
{
    private readonly IUserRegistrationService _userRegistrationService = userRegistrationService;

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegister userRegister)
    {
        var result = await _userRegistrationService.RegisterAsync(userRegister);
        return this.ToActionResult(result);
    }
}
