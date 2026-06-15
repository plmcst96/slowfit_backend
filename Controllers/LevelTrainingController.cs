using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/level")]
[ApiController]
public class LevelTrainingController(ICrudService<LevelTrainingRes> service) : ControllerBase
{
    private readonly ICrudService<LevelTrainingRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => this.ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateLevel([FromBody] LevelTrainingRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLevele(int id, [FromBody] LevelTrainingRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLevel(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
