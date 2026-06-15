using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/billing")]
[ApiController]
public class BillingController(IBillingService billingService) : ControllerBase
{
    private readonly IBillingService _billingService = billingService;

    [HttpGet]
    public async Task<IActionResult> Get() => this.ToActionResult(await _billingService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => this.ToActionResult(await _billingService.GetByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetBillingByUserId(int userId) => this.ToActionResult(await _billingService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> CreatBilling([FromBody] BillingRes request) => this.ToActionResult(await _billingService.CreateAsync(request));
}
