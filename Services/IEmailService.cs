namespace slowfit.Services;

public interface IEmailService
{
    /// <summary>
    /// Invia al personal trainer l'email con il link per impostare la password del profilo.
    /// Restituisce true se l'invio è andato a buon fine.
    /// </summary>
    Task<bool> SendPasswordSetupEmailAsync(string toEmail, string recipientName, string setupLink);
}
