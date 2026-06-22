using System.Net.Http.Headers;
using System.Text;

namespace slowfit.Services;

/// <summary>
/// Invio email tramite l'API HTTP di Mailgun. La stessa <see cref="IHttpClientFactory"/>
/// usata per le push Firebase viene riutilizzata anche qui.
/// </summary>
public sealed class EmailService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task<bool> SendPasswordSetupEmailAsync(string toEmail, string recipientName, string setupLink)
    {
        var displayName = string.IsNullOrWhiteSpace(recipientName) ? "Personal Trainer" : recipientName.Trim();
        // Oggetto personalizzato sul singolo PT, così ogni email risulta specifica per il destinatario creato.
        var subject = $"{displayName}, attiva il tuo profilo SlowFit";

        var text = $"""
            Ciao {displayName},

            il tuo profilo SlowFit è stato creato. Per accedere imposta la tua password cliccando sul link seguente:

            {setupLink}

            Per motivi di sicurezza il link ha una validità limitata. Se non hai richiesto questo account puoi ignorare questa email.

            Il team SlowFit
            """;

        var html = $"""
            <p>Ciao {displayName},</p>
            <p>il tuo profilo SlowFit è stato creato. Per accedere imposta la tua password cliccando sul pulsante seguente:</p>
            <p><a href="{setupLink}" style="display:inline-block;padding:12px 20px;background-color:#1f7a4d;color:#ffffff;text-decoration:none;border-radius:6px;">Imposta la password</a></p>
            <p>Se il pulsante non funziona, copia e incolla questo link nel browser:<br/><a href="{setupLink}">{setupLink}</a></p>
            <p>Per motivi di sicurezza il link ha una validità limitata. Se non hai richiesto questo account puoi ignorare questa email.</p>
            <p>Il team SlowFit</p>
            """;

        return await SendAsync(toEmail, subject, text, html);
    }

    private async Task<bool> SendAsync(string toEmail, string subject, string text, string html)
    {
        var apiKey = _configuration["Mailgun:ApiKey"];
        var domain = _configuration["Mailgun:Domain"];

        if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(domain))
        {
            _logger.LogWarning("Mailgun non è configurato (Mailgun:ApiKey / Mailgun:Domain mancanti): email a {ToEmail} non inviata.", toEmail);
            return false;
        }

        var baseUrl = (_configuration["Mailgun:BaseUrl"] ?? "https://api.mailgun.net").TrimEnd('/');
        var fromEmail = _configuration["Mailgun:FromEmail"] ?? $"no-reply@{domain}";
        var fromName = _configuration["Mailgun:FromName"] ?? "SlowFit";

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/v3/{domain}/messages")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["from"] = $"{fromName} <{fromEmail}>",
                ["to"] = toEmail,
                ["subject"] = subject,
                ["text"] = text,
                ["html"] = html
            })
        };

        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        try
        {
            var response = await _httpClientFactory.CreateClient().SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var body = await response.Content.ReadAsStringAsync();
            _logger.LogError("Invio email Mailgun a {ToEmail} fallito ({StatusCode}): {Body}", toEmail, (int)response.StatusCode, body);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'invio dell'email Mailgun a {ToEmail}.", toEmail);
            return false;
        }
    }
}
