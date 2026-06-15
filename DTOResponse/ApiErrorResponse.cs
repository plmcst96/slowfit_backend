namespace slowfit.DTOResponse;

public sealed class ApiErrorResponse
{
    public string Code { get; init; } = null!;

    public string Message { get; init; } = null!;

    public string? TraceId { get; init; }
}
