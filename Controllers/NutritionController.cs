using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/nutrition")]
[ApiController]
public class NutritionController(INutritionService nutritionService) : ControllerBase
{
    private readonly INutritionService _nutritionService = nutritionService;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _nutritionService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        this.ToActionResult(await _nutritionService.GetByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetByUser(int userId) =>
        this.ToActionResult(await _nutritionService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> CreateNutrition([FromBody] NutritionResPost request) =>
        this.ToActionResult(await _nutritionService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNutrition(int id, [FromBody] NutritionResPost request) =>
        this.ToActionResult(await _nutritionService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNutrition(int id) =>
        this.ToActionResult(await _nutritionService.DeleteAsync(id));
}
