using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[AllowAnonymous]
[Route("slowFit/quiz")]
[ApiController]
public class QuizController(IQuizService quizService) : ControllerBase
{
    private readonly IQuizService _quizService = quizService;

    [HttpGet]
    public async Task<IActionResult> GetQuizzes([FromQuery] string? type) => this.ToActionResult(await _quizService.GetAllAsync(type));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuiz(int id) => this.ToActionResult(await _quizService.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizUserRes request) => this.ToActionResult(await _quizService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuiz(int id, [FromBody] QuizUserRes request) => this.ToActionResult(await _quizService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(int id) => this.ToActionResult(await _quizService.DeleteAsync(id));
}
