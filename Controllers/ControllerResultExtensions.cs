using Microsoft.AspNetCore.Mvc;
using slowfit.DTOResponse;
using slowfit.Services;
using System.Net;

namespace slowfit.Controllers;

public static class ControllerResultExtensions
{
    public static ObjectResult ApiError(
        this ControllerBase controller,
        HttpStatusCode statusCode,
        string code,
        string message)
    {
        return new ObjectResult(new ApiErrorResponse
        {
            Code = code,
            Message = message,
            TraceId = controller.HttpContext.TraceIdentifier
        })
        {
            StatusCode = (int)statusCode
        };
    }

    public static ObjectResult ApiBadRequest(this ControllerBase controller, string code, string message) =>
        controller.ApiError(HttpStatusCode.BadRequest, code, message);

    public static ObjectResult ApiNotFound(this ControllerBase controller, string code, string message) =>
        controller.ApiError(HttpStatusCode.NotFound, code, message);

    public static ObjectResult ApiUnauthorized(this ControllerBase controller, string code = "unauthorized", string message = "Sessione scaduta o non valida. Effettua di nuovo il login.") =>
        controller.ApiError(HttpStatusCode.Unauthorized, code, message);

    public static ObjectResult ApiServerError(this ControllerBase controller, string code = "server_error", string message = "Si è verificato un errore del server. Riprova tra poco.") =>
        controller.ApiError(HttpStatusCode.InternalServerError, code, message);

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
