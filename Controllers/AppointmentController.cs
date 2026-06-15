using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;

namespace slowfit.Controllers;

[Route("slowFit/appointment")]
[ApiController]
public class AppointmentController(IAppointmentService appointmentService) : ControllerBase
{
    private readonly IAppointmentService _appointmentService = appointmentService;

    [HttpGet]
    public async Task<IActionResult> Get() => this.ToActionResult(await _appointmentService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleAppointment(int id) => this.ToActionResult(await _appointmentService.GetResponseByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetAppointmentByUserId(int userId) => this.ToActionResult(await _appointmentService.GetByUserAsync(userId));

    [HttpGet("byDate/{date}")]
    public async Task<IActionResult> GetAppointmentByDate(DateTime date) => this.ToActionResult(await _appointmentService.GetByDateAsync(date));

    [HttpGet("byPT/{ptId}")]
    public async Task<IActionResult> GetAppointmentByPtId(int ptId) => this.ToActionResult(await _appointmentService.GetByPtAsync(ptId));

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRes request) => this.ToActionResult(await _appointmentService.CreateAsync(request));

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentRes request) => this.ToActionResult(await _appointmentService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id) => this.ToActionResult(await _appointmentService.DeleteAsync(id));
}
