using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/order")]
[ApiController]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _orderService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleOrder(int id) => this.ToActionResult(await _orderService.GetByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetOrderByUserId(int userId) => this.ToActionResult(await _orderService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRes request) => this.ToActionResult(await _orderService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderRes request) => this.ToActionResult(await _orderService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id) => this.ToActionResult(await _orderService.DeleteAsync(id));
}
