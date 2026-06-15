using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/exercise")]
[ApiController]
public class DetailExerciseController(IDetailExerciseService detailExerciseService) : ControllerBase
{
    private readonly IDetailExerciseService _detailExerciseService = detailExerciseService;

    [HttpGet("detail")]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _detailExerciseService.GetAllAsync());

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetSingleDetail(int id) => this.ToActionResult(await _detailExerciseService.GetByIdAsync(id));

    [HttpGet("detailTraining/{trainingId}")]
    public async Task<IActionResult> GetSingleDetailByTraining(int trainingId) => this.ToActionResult(await _detailExerciseService.GetByTrainingAsync(trainingId));

    [HttpPost("detail")]
    public async Task<IActionResult> CreateDetail([FromBody] DetailExerciseRes request) => this.ToActionResult(await _detailExerciseService.CreateAsync(request));

    [HttpPut("detail/{id}")]
    public async Task<IActionResult> UpdateDetail(int id, [FromBody] DetailExerciseRes request) => this.ToActionResult(await _detailExerciseService.UpdateAsync(id, request));

    [HttpDelete("detail/{id}")]
    public async Task<IActionResult> DeleteDetail(int id) => this.ToActionResult(await _detailExerciseService.DeleteAsync(id));
}
