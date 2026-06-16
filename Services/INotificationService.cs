using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public interface INotificationService
{
    Task<ServiceResult<object>> UpdateFcmTokenAsync(UpdateFcmTokenRequest request);

    Task<ServiceResult<NotificationSendResponse>> NotifyTrainerByClientAsync(ClientToTrainerNotificationRequest request);

    Task<ServiceResult<NotificationSendResponse>> NotifyClientByTrainerAsync(TrainerToClientNotificationRequest request);

    Task<ServiceResult<NotificationsResponse>> GetByUserAsync(int userId);

    Task<ServiceResult<object>> DeleteAsync(int notificationId, int requesterId);
}
