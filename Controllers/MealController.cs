using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/meal")]
[ApiController]
public class MealController(IMealService mealService) : ControllerBase
{
    private readonly IMealService _mealService = mealService;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        this.ToActionResult(await _mealService.GetAllAsync());

    [HttpGet("{id}/withIngredients")]
    public async Task<IActionResult> GetMealWithIngredients(int id) =>
        this.ToActionResult(await _mealService.GetMealWithIngredientsAsync(id));

    [HttpGet("byIngredient/{ingredientId}")]
    public async Task<IActionResult> GetMealsByIngredient(int ingredientId) =>
        this.ToActionResult(await _mealService.GetByIngredientAsync(ingredientId));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleMeal(int id) =>
        this.ToActionResult(await _mealService.GetByIdAsync(id));

    [HttpGet("byCategory/{categoryId}")]
    public async Task<IActionResult> GetMealsByCategory(int categoryId) =>
        this.ToActionResult(await _mealService.GetByCategoryAsync(categoryId));

    [HttpGet("byDay/{dayId}")]
    public async Task<IActionResult> GetMealsByDay(int dayId) =>
        this.ToActionResult(await _mealService.GetByDayAsync(dayId));

    [HttpPost]
    public async Task<IActionResult> CreateMealWithIngredients([FromBody] MealResWithIngredients request) =>
        this.ToActionResult(await _mealService.CreateWithIngredientsAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMealWithIngredients(int id, [FromBody] MealResWithIngredients request) =>
        this.ToActionResult(await _mealService.UpdateWithIngredientsAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeal(int id) =>
        this.ToActionResult(await _mealService.DeleteAsync(id));
}
