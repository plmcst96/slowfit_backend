using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/response")]
[ApiController]
public class ResponseQuizController(IResponseQuizService responseQuizService) : ControllerBase
{
    private readonly IResponseQuizService _responseQuizService = responseQuizService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _responseQuizService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleResponse(int id) => this.ToActionResult(await _responseQuizService.GetByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetResponseQuizByUserId(int userId) => this.ToActionResult(await _responseQuizService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ResponseQuizRes request) => this.ToActionResult(await _responseQuizService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResponse(int id, [FromBody] ResponseQuizRes request) => this.ToActionResult(await _responseQuizService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(int id) => this.ToActionResult(await _responseQuizService.DeleteAsync(id));
}
