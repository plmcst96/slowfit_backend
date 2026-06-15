using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/input")]
[ApiController]
public class InputTypeController(ICrudService<InputTypeRes> service) : ControllerBase
{
    private readonly ICrudService<InputTypeRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetInputTypes() => this.ToActionResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInputTypeById(int id) => this.ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateInputType([FromBody] InputTypeRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInputType(int id, [FromBody] InputTypeRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInputType(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
