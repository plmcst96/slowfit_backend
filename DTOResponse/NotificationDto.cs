namespace slowfit.DTOResponse;

public sealed class NotificationDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public IReadOnlyDictionary<string, string>? Data { get; set; }

    public DateTime CreatedAt { get; set; }
}
