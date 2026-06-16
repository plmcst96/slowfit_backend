using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/progressNutrition")]
[ApiController]
public class ProgressNutritionController(IProgressNutritionService progressNutritionService) : ControllerBase
{
    private readonly IProgressNutritionService _progressNutritionService = progressNutritionService;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!User.IsPersonalTrainer()) return Forbid();
        return this.ToActionResult(await _progressNutritionService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _progressNutritionService.GetByIdAsync(id);
        if (result.IsSuccess && result.Value != null && !User.CanAccessUser(result.Value.UserId)) return Forbid();
        return this.ToActionResult(result);
    }

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        if (!User.CanAccessUser(userId)) return Forbid();
        return this.ToActionResult(await _progressNutritionService.GetByUserAsync(userId));
    }

    [HttpGet("byNutrition/{nutritionId}")]
    public async Task<IActionResult> GetByNutrition(int nutritionId)
    {
        if (!User.IsPersonalTrainer()) return Forbid();
        return this.ToActionResult(await _progressNutritionService.GetByNutritionAsync(nutritionId));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProgressNutritionRes request)
    {
        if (request == null) return BadRequest();
        if (!User.CanAccessUser(request.UserId)) return Forbid();
        return this.ToActionResult(await _progressNutritionService.CreateAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProgressNutritionRes request)
    {
        if (request == null) return BadRequest();
        if (!User.CanAccessUser(request.UserId)) return Forbid();
        return this.ToActionResult(await _progressNutritionService.UpdateAsync(id, request));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _progressNutritionService.GetByIdAsync(id);
        if (result.IsSuccess && result.Value != null && !User.CanAccessUser(result.Value.UserId)) return Forbid();
        return this.ToActionResult(await _progressNutritionService.DeleteAsync(id));
    }
}
