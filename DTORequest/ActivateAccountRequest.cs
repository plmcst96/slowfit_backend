namespace slowfit.DTORequest
{
    /// <summary>
    /// Richiesta anonima di attivazione profilo: il personal trainer imposta la password
    /// usando il token monouso ricevuto via email.
    /// </summary>
    public class ActivateAccountRequest
    {
        public string Token { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
