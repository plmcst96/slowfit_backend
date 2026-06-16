using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class NotificationService(SlowFitContext slowFitContext, IHttpClientFactory httpClientFactory, IConfiguration configuration) : INotificationService
{
    private readonly SlowFitContext _slowFitContext = slowFitContext;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ServiceResult<object>> UpdateFcmTokenAsync(UpdateFcmTokenRequest request)
    {
        if (request == null || request.UserId <= 0 || string.IsNullOrWhiteSpace(request.FcmToken))
        {
            return ServiceResult<object>.BadRequest("invalid_fcm_token", "User id and FCM token are required.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
        if (user == null)
        {
            return ServiceResult<object>.NotFound("user_not_found", "User not found.");
        }

        user.FcmToken = request.FcmToken.Trim();
        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<NotificationSendResponse>> NotifyTrainerByClientAsync(ClientToTrainerNotificationRequest request)
    {
        if (request == null || request.ClientId <= 0 || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
        {
            return ServiceResult<NotificationSendResponse>.BadRequest("invalid_notification", "Client id, title and body are required.");
        }

        var client = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == request.ClientId);
        if (client == null)
        {
            return ServiceResult<NotificationSendResponse>.NotFound("client_not_found", "Client not found.");
        }

        if (!client.PtId.HasValue)
        {
            return ServiceResult<NotificationSendResponse>.BadRequest("trainer_not_assigned", "Client has no assigned trainer.");
        }

        var data = ToStringDictionary(request.Data);
        data["type"] = "client_to_trainer";
        data["clientId"] = request.ClientId.ToString();

        return await CreateAndSendAsync(client.PtId.Value, "trainer", request.Title, request.Body, data);
    }

    public async Task<ServiceResult<NotificationSendResponse>> NotifyClientByTrainerAsync(TrainerToClientNotificationRequest request)
    {
        if (request == null || request.ClientId <= 0 || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
        {
            return ServiceResult<NotificationSendResponse>.BadRequest("invalid_notification", "Client id, title and body are required.");
        }

        var data = ToStringDictionary(request.Data);
        data["type"] = "trainer_to_client";
        data["clientId"] = request.ClientId.ToString();

        return await CreateAndSendAsync(request.ClientId, "client", request.Title, request.Body, data);
    }

    public async Task<ServiceResult<NotificationsResponse>> GetByUserAsync(int userId)
    {
        var notifications = await _slowFitContext.NotificationsFires
            .AsNoTracking()
            .Where(n => n.ReceiverId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                CreatedAt = n.CreatedAt,
                Data = DeserializeData(n.Data)
            })
            .ToListAsync();

        return ServiceResult<NotificationsResponse>.Ok(new NotificationsResponse { Notifications = notifications });
    }

    public async Task<ServiceResult<object>> DeleteAsync(int notificationId, int requesterId)
    {
        var notification = await _slowFitContext.NotificationsFires.FirstOrDefaultAsync(n => n.Id == notificationId);
        if (notification == null)
        {
            return ServiceResult<object>.NotFound("notification_not_found", "Notification not found.");
        }

        if (notification.ReceiverId != requesterId)
        {
            return ServiceResult<object>.Unauthorized("notification_forbidden", "You cannot delete this notification.");
        }

        _slowFitContext.NotificationsFires.Remove(notification);
        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    private async Task<ServiceResult<NotificationSendResponse>> CreateAndSendAsync(int receiverId, string receiverRole, string title, string body, IReadOnlyDictionary<string, string> data)
    {
        var receiver = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == receiverId);
        if (receiver == null)
        {
            return ServiceResult<NotificationSendResponse>.NotFound("receiver_not_found", "Notification receiver not found.");
        }

        var notification = new NotificationsFire
        {
            ReceiverId = receiver.UserId,
            ReceiverRole = receiverRole,
            Title = title.Trim(),
            Body = body.Trim(),
            Data = data.Count == 0 ? null : JsonSerializer.Serialize(data),
            CreatedAt = DateTime.UtcNow
        };

        _slowFitContext.NotificationsFires.Add(notification);
        await _slowFitContext.SaveChangesAsync();

        var (pushSent, message) = await SendFirebasePushAsync(receiver.FcmToken, notification.Title, notification.Body, data);
        return ServiceResult<NotificationSendResponse>.Ok(new NotificationSendResponse
        {
            NotificationId = notification.Id,
            PushSent = pushSent,
            Message = message
        });
    }

    private async Task<(bool Sent, string Message)> SendFirebasePushAsync(string? fcmToken, string title, string body, IReadOnlyDictionary<string, string> data)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
        {
            return (false, "Receiver has no FCM token.");
        }

        var firebaseConfig = LoadFirebaseConfig();
        if (firebaseConfig == null)
        {
            return (false, "Firebase service account is not configured.");
        }

        var accessToken = await GetFirebaseAccessTokenAsync(firebaseConfig);

        var payload = JsonSerializer.Serialize(new
        {
            message = new
            {
                token = fcmToken,
                notification = new { title, body },
                data
            }
        });

        var request = new HttpRequestMessage(HttpMethod.Post, $"https://fcm.googleapis.com/v1/projects/{firebaseConfig.ProjectId}/messages:send")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClientFactory.CreateClient().SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return (true, "Push sent.");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        return (false, string.IsNullOrWhiteSpace(responseBody) ? "Firebase push failed." : responseBody);
    }

    private FirebaseServiceAccount? LoadFirebaseConfig()
    {
        var projectId = _configuration["Firebase:ProjectId"];
        var clientEmail = _configuration["Firebase:ClientEmail"];
        var privateKey = _configuration["Firebase:PrivateKey"];

        if (!string.IsNullOrWhiteSpace(projectId) && !string.IsNullOrWhiteSpace(clientEmail) && !string.IsNullOrWhiteSpace(privateKey))
        {
            return new FirebaseServiceAccount(projectId, clientEmail, NormalizePrivateKey(privateKey));
        }

        var serviceAccountJson = _configuration["Firebase:ServiceAccountJson"];
        var serviceAccountPath = _configuration["Firebase:ServiceAccountPath"];

        if (string.IsNullOrWhiteSpace(serviceAccountJson) && !string.IsNullOrWhiteSpace(serviceAccountPath) && File.Exists(serviceAccountPath))
        {
            serviceAccountJson = File.ReadAllText(serviceAccountPath);
        }

        if (string.IsNullOrWhiteSpace(serviceAccountJson))
        {
            return null;
        }

        using var document = JsonDocument.Parse(serviceAccountJson);
        var root = document.RootElement;
        projectId = root.GetProperty("project_id").GetString();
        clientEmail = root.GetProperty("client_email").GetString();
        privateKey = root.GetProperty("private_key").GetString();

        return string.IsNullOrWhiteSpace(projectId) || string.IsNullOrWhiteSpace(clientEmail) || string.IsNullOrWhiteSpace(privateKey)
            ? null
            : new FirebaseServiceAccount(projectId, clientEmail, NormalizePrivateKey(privateKey));
    }

    private async Task<string> GetFirebaseAccessTokenAsync(FirebaseServiceAccount firebaseConfig)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(firebaseConfig.PrivateKey);

        var now = DateTimeOffset.UtcNow;
        var token = new JwtSecurityToken(
            issuer: firebaseConfig.ClientEmail,
            audience: "https://oauth2.googleapis.com/token",
            claims: [new Claim("scope", "https://www.googleapis.com/auth/firebase.messaging")],
            notBefore: now.UtcDateTime,
            expires: now.AddMinutes(55).UtcDateTime,
            signingCredentials: new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256));

        var assertion = new JwtSecurityTokenHandler().WriteToken(token);
        var response = await _httpClientFactory.CreateClient().PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
            ["assertion"] = assertion
        }));

        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"Firebase authentication failed: {responseBody}");
        }

        using var document = JsonDocument.Parse(responseBody);
        return document.RootElement.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("Firebase authentication response did not include an access token.");
    }

    private static string NormalizePrivateKey(string privateKey) => privateKey.Replace("\\n", "\n");

    private static Dictionary<string, string> ToStringDictionary(JsonElement? data)
    {
        if (!data.HasValue || data.Value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            return [];
        }

        if (data.Value.ValueKind != JsonValueKind.Object)
        {
            return [];
        }

        return data.Value.EnumerateObject().ToDictionary(property => property.Name, property => property.Value.ValueKind switch
        {
            JsonValueKind.String => property.Value.GetString() ?? string.Empty,
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => property.Value.ToString(),
            _ => property.Value.GetRawText()
        });
    }

    private static IReadOnlyDictionary<string, string>? DeserializeData(string? data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, string>>(data);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private sealed record FirebaseServiceAccount(string ProjectId, string ClientEmail, string PrivateKey);
}
