namespace slowfit.DTOResponse;

public sealed class NotificationsResponse
{
    public IReadOnlyList<NotificationDto> Notifications { get; set; } = [];
}
