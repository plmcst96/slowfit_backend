using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/measure")]
[ApiController]
public class MeasureController(IMeasureService measureService) : ControllerBase
{
    private readonly IMeasureService _measureService = measureService;

    [HttpGet]
    public async Task<IActionResult> Get() => this.ToActionResult(await _measureService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeasureById(int id) => this.ToActionResult(await _measureService.GetByIdAsync(id));

    [HttpGet("byDate")]
    public async Task<IActionResult> GetMeasureByDate([FromQuery] DateTime date) => this.ToActionResult(await _measureService.GetByDateAsync(date));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetMeasuresByUserId(int userId) => this.ToActionResult(await _measureService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> CreateMeasure([FromBody] MeasureRes request) => this.ToActionResult(await _measureService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMeasure(int id, [FromBody] MeasureRes request) => this.ToActionResult(await _measureService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeasure(int id) => this.ToActionResult(await _measureService.DeleteAsync(id));
}
