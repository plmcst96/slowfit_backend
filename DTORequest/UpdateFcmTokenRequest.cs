namespace slowfit.DTORequest;

public sealed class UpdateFcmTokenRequest
{
    public int UserId { get; set; }

    public string FcmToken { get; set; } = null!;
}
