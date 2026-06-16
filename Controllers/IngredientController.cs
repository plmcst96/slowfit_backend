using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/ingredient")]
[ApiController]
public class IngredientController(IIngredientService ingredientService) : ControllerBase
{
    private readonly IIngredientService _ingredientService = ingredientService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _ingredientService.GetAllAsync());

    [HttpGet("search")]
    public async Task<IActionResult> GetBySearchQuery([FromQuery] string name) => this.ToActionResult(await _ingredientService.SearchAsync(name));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleIngredient(int id) => this.ToActionResult(await _ingredientService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateIngredient([FromBody] IngredientRes request) => this.ToActionResult(await _ingredientService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientRes request) => this.ToActionResult(await _ingredientService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIngredient(int id) => this.ToActionResult(await _ingredientService.DeleteAsync(id));
}
