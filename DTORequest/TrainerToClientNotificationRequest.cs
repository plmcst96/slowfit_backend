using System.Text.Json;

namespace slowfit.DTORequest;

public sealed class TrainerToClientNotificationRequest
{
    public int ClientId { get; set; }

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public JsonElement? Data { get; set; }
}
