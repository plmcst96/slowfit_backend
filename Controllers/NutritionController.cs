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
        this.ToActionResult(await _nutritionService.GetAllAsync(User));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) =>
        this.ToActionResult(await _nutritionService.GetByIdAsync(User, id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetByUser(int userId) =>
        this.ToActionResult(await _nutritionService.GetByUserAsync(User, userId));

    [HttpPost]
    public async Task<IActionResult> CreateNutrition([FromBody] NutritionResPost request) =>
        this.ToActionResult(await _nutritionService.CreateAsync(User, request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNutrition(int id, [FromBody] NutritionResPost request) =>
        this.ToActionResult(await _nutritionService.UpdateAsync(User, id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNutrition(int id) =>
        this.ToActionResult(await _nutritionService.DeleteAsync(User, id));
}
