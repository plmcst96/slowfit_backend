using Microsoft.AspNetCore.Mvc;
using slowfit.DTORequest;
using slowfit.Services;
using System.Text.Json;

namespace slowfit.Controllers;

[Route("slowFit/response")]
[ApiController]
public class ResponseQuizController(IResponseQuizService responseQuizService) : ControllerBase
{
    private readonly IResponseQuizService _responseQuizService = responseQuizService;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<IActionResult> GetAll() => this.ToActionResult(await _responseQuizService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSingleResponse(int id) => this.ToActionResult(await _responseQuizService.GetByIdAsync(id));

    [HttpGet("byUser/{userId}")]
    public async Task<IActionResult> GetResponseQuizByUserId(int userId) => this.ToActionResult(await _responseQuizService.GetByUserAsync(userId));

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] JsonElement request)
    {
        try
        {
            if (request.ValueKind == JsonValueKind.Array)
            {
                var responses = request.Deserialize<List<ResponseQuizRes>>(JsonOptions) ?? [];
                return this.ToActionResult(await _responseQuizService.CreateManyAsync(responses));
            }

            var response = request.Deserialize<ResponseQuizRes>(JsonOptions);
            return this.ToActionResult(await _responseQuizService.CreateAsync(response!));
        }
        catch (JsonException)
        {
            return this.ApiBadRequest("invalid_response_quiz", "Le risposte del quiz non sono valide. Riprova.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResponse(int id, [FromBody] ResponseQuizRes request) => this.ToActionResult(await _responseQuizService.UpdateAsync(id, request));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResponse(int id) => this.ToActionResult(await _responseQuizService.DeleteAsync(id));
}
