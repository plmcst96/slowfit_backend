using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/progress")]
[ApiController]
public class ProgressController(IProgressTrainingService progressTrainingService) : ControllerBase
{
    private readonly IProgressTrainingService _progressTrainingService = progressTrainingService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!User.IsPersonalTrainer()) return Forbid();
        return this.ToActionResult(await _progressTrainingService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _progressTrainingService.GetByIdAsync(id);
        if (result.IsSuccess && result.Value != null && !User.CanAccessUser(result.Value.UserId)) return Forbid();
        return this.ToActionResult(result);
    }

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        if (!User.CanAccessUser(userId)) return Forbid();
        return this.ToActionResult(await _progressTrainingService.GetByUserAsync(userId));
    }

    [HttpGet("byTraining/{trainingId}")]
    public async Task<IActionResult> GetByTraining(int trainingId)
    {
        if (!User.IsPersonalTrainer()) return Forbid();
        return this.ToActionResult(await _progressTrainingService.GetByTrainingAsync(trainingId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProgressTrainingRes request)
    {
        if (request == null) return this.ApiBadRequest("invalid_progress_training", "I dati del progresso allenamento sono obbligatori.");
        if (!User.CanAccessUser(request.UserId)) return Forbid();
        return this.ToActionResult(await _progressTrainingService.CreateAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProgressTrainingRes request)
    {
        if (request == null) return this.ApiBadRequest("invalid_progress_training", "I dati del progresso allenamento sono obbligatori.");
        if (!User.CanAccessUser(request.UserId)) return Forbid();
        return this.ToActionResult(await _progressTrainingService.UpdateAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _progressTrainingService.GetByIdAsync(id);
        if (result.IsSuccess && result.Value != null && !User.CanAccessUser(result.Value.UserId)) return Forbid();
        return this.ToActionResult(await _progressTrainingService.DeleteAsync(id));
    }
}
