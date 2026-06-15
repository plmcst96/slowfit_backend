using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/product")]
[ApiController]
public class ProductController(ICrudService<ProductRes> service) : ControllerBase
{
    private readonly ICrudService<ProductRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAllProducts() => this.ToActionResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleProduct(int id) => this.ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] ProductRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
