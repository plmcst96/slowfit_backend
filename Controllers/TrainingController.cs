using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/training")]
[ApiController]
public class TrainingController(ITrainingService trainingService) : ControllerBase
{
    private readonly ITrainingService _trainingService = trainingService;

    [HttpGet]
    public async Task<IActionResult> GetTrainings() =>
        this.ToActionResult(await _trainingService.GetAllAsync(User));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrainingById(int id) =>
        this.ToActionResult(await _trainingService.GetByIdAsync(User, id));

    [HttpGet("byDate")]
    public async Task<IActionResult> GetTrainingByDate([FromQuery] DateTime date) =>
        this.ToActionResult(await _trainingService.GetByDateAsync(User, date));

    [HttpGet("byDateRange")]
    public async Task<IActionResult> GetTrainingByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate) =>
        this.ToActionResult(await _trainingService.GetByDateRangeAsync(User, startDate, endDate));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetTrainingByUserId(int userId) =>
        this.ToActionResult(await _trainingService.GetByUserAsync(User, userId));

    [HttpPost]
    public async Task<IActionResult> CreateTrainingDet([FromBody] TrainingRes request) =>
        this.ToActionResult(await _trainingService.CreateAsync(User, request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTraining(int id, [FromBody] TrainingRes updatedTraining) =>
        this.ToActionResult(await _trainingService.UpdateAsync(User, id, updatedTraining));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTraining(int id) =>
        this.ToActionResult(await _trainingService.DeleteAsync(User, id));
}
