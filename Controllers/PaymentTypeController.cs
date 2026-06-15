using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/payment")]
[ApiController]
public class PaymentTypeController(ICrudService<PaymentTypeRes> service) : ControllerBase
{
    private readonly ICrudService<PaymentTypeRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PaymentTypeRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] PaymentTypeRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
