using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[AllowAnonymous]
[Route("slowFit/answer")]
[ApiController]
public class AnswerCotroller(IAnswerService answerService) : ControllerBase
{
    private readonly IAnswerService _answerService = answerService;

    [HttpGet]
    public async Task<IActionResult> GetAnswers() => this.ToActionResult(await _answerService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAnswerById(int id) => this.ToActionResult(await _answerService.GetByIdAsync(id));

    [HttpGet("byQuestion/{questionId}")]
    public async Task<IActionResult> GetAnswersByQuestionId(int questionId) => this.ToActionResult(await _answerService.GetByQuestionAsync(questionId));

    [HttpPost]
    public async Task<IActionResult> CreateAnswer([FromBody] AnswerRes request) => this.ToActionResult(await _answerService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAnswer(int id, [FromBody] AnswerRes request) => this.ToActionResult(await _answerService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnswer(int id) => this.ToActionResult(await _answerService.DeleteAsync(id));
}
