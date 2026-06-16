using Microsoft.AspNetCore.Mvc;
using slowfit.DTOResponse;
using slowfit.Services;

namespace slowfit.Controllers;

public static class ControllerResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
    {
        if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            return controller.NoContent();
        }

        if (result.IsSuccess)
        {
            // GET di un singolo oggetto non trovato -> 200 con {} invece di un errore.
            return controller.StatusCode((int)result.StatusCode, result.Value ?? (object)new { });
        }

        return controller.StatusCode((int)result.StatusCode, new ApiErrorResponse
        {
            Code = result.ErrorCode ?? "error",
            Message = result.ErrorMessage ?? "An error occurred.",
            TraceId = controller.HttpContext.TraceIdentifier
        });
    }
}
