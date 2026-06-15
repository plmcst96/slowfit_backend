using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[AllowAnonymous]
[Route("slowFit/question")]
[ApiController]
public class QuestionController(ICrudService<QuestionRes> service) : ControllerBase
{
    private readonly ICrudService<QuestionRes> _service = service;

    [HttpGet]
    public async Task<IActionResult> GetQuestions() => this.ToActionResult(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuestionById(int id) => this.ToActionResult(await _service.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateQuestion([FromBody] QuestionRes request) => this.ToActionResult(await _service.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] QuestionRes request) => this.ToActionResult(await _service.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(int id) => this.ToActionResult(await _service.DeleteAsync(id));
}
