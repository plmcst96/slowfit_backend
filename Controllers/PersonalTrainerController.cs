using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/pt")]
[ApiController]
public class PersonalTrainerController(IPersonalTrainerService personalTrainerService) : ControllerBase
{
    private readonly IPersonalTrainerService _personalTrainerService = personalTrainerService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!User.IsSuperAdmin()) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (!User.IsSuperAdmin() && User.GetUserId() != id) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonalTrainerReq request)
    {
        if (!User.IsSuperAdmin()) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.CreateAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PersonalTrainerReq request)
    {
        if (!User.IsSuperAdmin() && User.GetUserId() != id) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.UpdateAsync(id, request));
    }

    [HttpPatch("{id}/password")]
    public async Task<IActionResult> SetPassword(int id, [FromBody] SetPasswordRequest request)
    {
        // Reset diretto della password (super admin o PT autenticato sul proprio profilo).
        if (!User.IsSuperAdmin() && User.GetUserId() != id) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.SetPasswordAsync(id, request));
    }

    // Endpoint anonimo richiamato dal link di attivazione inviato via email: il PT imposta la password col token.
    [AllowAnonymous]
    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] ActivateAccountRequest request)
    {
        return this.ToActionResult(await _personalTrainerService.ActivateAsync(request));
    }

    // Re-invio dell'email di attivazione (solo super admin), ad esempio se il link è scaduto o l'email non è arrivata.
    [HttpPost("{id}/resend-activation")]
    public async Task<IActionResult> ResendActivation(int id)
    {
        if (!User.IsSuperAdmin()) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.ResendActivationAsync(id));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!User.IsSuperAdmin() && User.GetUserId() != id) return this.ApiForbidden();
        return this.ToActionResult(await _personalTrainerService.DeleteAsync(id));
    }
}
