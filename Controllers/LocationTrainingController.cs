using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/location")]
[ApiController]
public class LocationTrainingController(ICrudService<LocationTrainingRes> service) : ControllerBase
{
    private readonly ICrudService<LocationTrainingRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> CreateLocationTraining([FromBody] LocationTrainingRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] LocationTrainingRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLocalTraining(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
