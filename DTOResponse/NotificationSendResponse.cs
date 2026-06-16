namespace slowfit.DTOResponse;

public sealed class NotificationSendResponse
{
    public int NotificationId { get; set; }

    public bool PushSent { get; set; }

    public string? Message { get; set; }
}
