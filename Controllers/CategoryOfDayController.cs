using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/category")]
[ApiController]
public class CategoryOfDayController(ICrudService<CategoryOfDayRes> service) : ControllerBase
{
    private readonly ICrudService<CategoryOfDayRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => this.ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CategoryOfDayRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CategoryOfDayRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
