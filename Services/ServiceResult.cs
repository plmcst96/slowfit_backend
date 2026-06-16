using System.Net;

namespace slowfit.Services;

public sealed class ServiceResult<T>
{
    private ServiceResult(HttpStatusCode statusCode, T? value = default, string? errorCode = null, string? errorMessage = null)
    {
        StatusCode = statusCode;
        Value = value;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public HttpStatusCode StatusCode { get; }

    public T? Value { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public bool IsSuccess => (int)StatusCode is >= 200 and <= 299;

    public static ServiceResult<T> Ok(T value) => new(HttpStatusCode.OK, value);

    public static ServiceResult<T> Created(T value) => new(HttpStatusCode.Created, value);

    public static ServiceResult<T> NoContent() => new(HttpStatusCode.NoContent);

    public static ServiceResult<T> BadRequest(string code, string message) => new(HttpStatusCode.BadRequest, errorCode: code, errorMessage: message);

    public static ServiceResult<T> Unauthorized(string code, string message) => new(HttpStatusCode.Unauthorized, errorCode: code, errorMessage: message);

    public static ServiceResult<T> Forbidden(string code, string message) => new(HttpStatusCode.Forbidden, errorCode: code, errorMessage: message);

    public static ServiceResult<T> NotFound(string code, string message) => new(HttpStatusCode.NotFound, errorCode: code, errorMessage: message);

    public static ServiceResult<T> Conflict(string code, string message) => new(HttpStatusCode.Conflict, errorCode: code, errorMessage: message);

    public static ServiceResult<T> Error(string code, string message) => new(HttpStatusCode.InternalServerError, errorCode: code, errorMessage: message);
}
