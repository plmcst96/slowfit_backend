using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/api/exercise")]
[ApiController]
public class ExerciseController(IExerciseService exerciseService) : ControllerBase
{
    private readonly IExerciseService _exerciseService = exerciseService;

    [HttpGet]
    public async Task<IActionResult> Get() => this.ToActionResult(await _exerciseService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleExercise(int id) => this.ToActionResult(await _exerciseService.GetByIdAsync(id));

    [HttpGet("exercise/{locationId}")]
    public async Task<IActionResult> GetExercisesByLocationId(int locationId) => this.ToActionResult(await _exerciseService.GetByLocationAsync(locationId));

    [HttpGet("exerciseByTraining/{trainingId}")]
    public async Task<IActionResult> GetExercisesByTypeTraining(int trainingId) => this.ToActionResult(await _exerciseService.GetByTypeTrainingAsync(trainingId));

    [HttpPost]
    public async Task<IActionResult> CreateExercise([FromBody] ExerciseRes request) => this.ToActionResult(await _exerciseService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExercise(int id, [FromBody] ExerciseRes request) => this.ToActionResult(await _exerciseService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExercise(int id) => this.ToActionResult(await _exerciseService.DeleteAsync(id));
}
