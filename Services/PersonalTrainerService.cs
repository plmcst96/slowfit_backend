using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using slowfit.Auth;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class PersonalTrainerService(SlowFitContext slowFitContext, IEmailService emailService, IConfiguration configuration) : IPersonalTrainerService
{
    private const int PersonalTrainerRoleId = ClaimsPrincipalExtensions.PersonalTrainerRoleId;
    private const int MinPasswordLength = 8;
    private static readonly TimeSpan PasswordSetupTokenLifetime = TimeSpan.FromDays(7);

    private readonly SlowFitContext _slowFitContext = slowFitContext;
    private readonly IEmailService _emailService = emailService;
    private readonly IConfiguration _configuration = configuration;
    private readonly PasswordHasher<PersonalTrainer> _passwordHasher = new();

    public async Task<ServiceResult<IReadOnlyList<PersonalTrainerResponse>>> GetAllAsync()
    {
        var trainers = await _slowFitContext.PersonalTrainers
            .AsNoTracking()
            .Select(p => ToResponse(p, p.Clients.Select(c => c.UserId).ToList()))
            .ToListAsync();

        return ServiceResult<IReadOnlyList<PersonalTrainerResponse>>.Ok(trainers);
    }

    public async Task<ServiceResult<PersonalTrainerResponse>> GetByIdAsync(int ptId)
    {
        var trainer = await _slowFitContext.PersonalTrainers
            .AsNoTracking()
            .Where(p => p.PtId == ptId)
            .Select(p => ToResponse(p, p.Clients.Select(c => c.UserId).ToList()))
            .FirstOrDefaultAsync();

        return trainer == null
            ? ServiceResult<PersonalTrainerResponse>.Ok(default!)
            : ServiceResult<PersonalTrainerResponse>.Ok(trainer);
    }

    public async Task<ServiceResult<PersonalTrainerResponse>> CreateAsync(PersonalTrainerReq request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.Surname) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.VatNumber) ||
            string.IsNullOrWhiteSpace(request.FiscalCode) ||
            string.IsNullOrWhiteSpace(request.PecEmail))
        {
            return ServiceResult<PersonalTrainerResponse>.BadRequest("invalid_trainer", "Compila i campi obbligatori: nome, cognome, email, P.IVA, codice fiscale e PEC.");
        }

        if (await IsEmailInUseAsync(request.Email, null))
        {
            return ServiceResult<PersonalTrainerResponse>.Conflict("email_in_use", "Questa email è già registrata.");
        }

        // Nessuna password in fase di creazione: verrà impostata dal PT tramite il flusso email.
        var trainer = new PersonalTrainer
        {
            FirstName = request.FirstName,
            Surname = request.Surname,
            Email = request.Email,
            Password = null,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Province = request.Province,
            ZipCode = request.ZipCode,
            BirthDate = request.BirthDate?.Date,
            ImageProfile = request.ImageProfile,
            Phone = request.Phone,
            VatNumber = request.VatNumber,
            FiscalCode = request.FiscalCode,
            SdiCode = request.SdiCode,
            PecEmail = request.PecEmail
        };

        // Genera il token monouso per l'attivazione e salvane solo l'hash.
        var setupToken = GeneratePasswordSetupToken(trainer);

        _slowFitContext.PersonalTrainers.Add(trainer);
        await _slowFitContext.SaveChangesAsync();

        // L'invio email non deve compromettere la creazione: se fallisce il super admin può
        // usare l'endpoint di re-invio. L'esito viene comunque tracciato nei log dell'EmailService.
        var emailSent = await _emailService.SendPasswordSetupEmailAsync(trainer.Email, $"{trainer.FirstName} {trainer.Surname}", BuildSetupLink(setupToken));

        var response = ToResponse(trainer, []);
        response.EmailSent = emailSent;
        return ServiceResult<PersonalTrainerResponse>.Created(response);
    }

    public async Task<ServiceResult<object>> UpdateAsync(int ptId, PersonalTrainerReq request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.Surname) ||
            string.IsNullOrWhiteSpace(request.Email))
        {
            return ServiceResult<object>.BadRequest("invalid_trainer", "I dati del personal trainer non sono validi.");
        }

        var trainer = await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.PtId == ptId);
        if (trainer == null) return ServiceResult<object>.NotFound("trainer_not_found", "Personal trainer non trovato.");

        if (await IsEmailInUseAsync(request.Email, ptId))
        {
            return ServiceResult<object>.Conflict("email_in_use", "Questa email è già registrata.");
        }

        trainer.FirstName = request.FirstName;
        trainer.Surname = request.Surname;
        trainer.Email = request.Email;
        trainer.Address = request.Address;
        trainer.City = request.City;
        trainer.Country = request.Country;
        trainer.Province = request.Province;
        trainer.ZipCode = request.ZipCode;
        trainer.BirthDate = request.BirthDate?.Date;
        trainer.ImageProfile = request.ImageProfile;
        trainer.Phone = request.Phone;
        trainer.VatNumber = request.VatNumber;
        trainer.FiscalCode = request.FiscalCode;
        trainer.SdiCode = request.SdiCode;
        trainer.PecEmail = request.PecEmail;

        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> SetPasswordAsync(int ptId, SetPasswordRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<object>.BadRequest("invalid_password", "La password non può essere vuota.");
        }

        var trainer = await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.PtId == ptId);
        if (trainer == null) return ServiceResult<object>.NotFound("trainer_not_found", "Personal trainer non trovato.");

        trainer.Password = _passwordHasher.HashPassword(trainer, request.Password);

        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> ActivateAsync(ActivateAccountRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<object>.BadRequest("invalid_activation", "Token e password sono obbligatori.");
        }

        if (request.Password.Length < MinPasswordLength)
        {
            return ServiceResult<object>.BadRequest("weak_password", $"La password deve contenere almeno {MinPasswordLength} caratteri.");
        }

        var tokenHash = HashToken(request.Token);
        var trainer = await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.PasswordSetupTokenHash == tokenHash);
        if (trainer == null)
        {
            return ServiceResult<object>.BadRequest("invalid_token", "Link di attivazione non valido.");
        }

        if (!trainer.PasswordSetupTokenExpiresAt.HasValue || trainer.PasswordSetupTokenExpiresAt <= DateTime.UtcNow)
        {
            return ServiceResult<object>.BadRequest("expired_token", "Il link di attivazione è scaduto. Richiedine uno nuovo.");
        }

        trainer.Password = _passwordHasher.HashPassword(trainer, request.Password);
        // Token monouso: invalidalo dopo l'uso.
        trainer.PasswordSetupTokenHash = null;
        trainer.PasswordSetupTokenExpiresAt = null;

        await _slowFitContext.SaveChangesAsync();
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> ResendActivationAsync(int ptId)
    {
        var trainer = await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.PtId == ptId);
        if (trainer == null) return ServiceResult<object>.NotFound("trainer_not_found", "Personal trainer non trovato.");

        var setupToken = GeneratePasswordSetupToken(trainer);
        await _slowFitContext.SaveChangesAsync();

        var sent = await _emailService.SendPasswordSetupEmailAsync(trainer.Email, $"{trainer.FirstName} {trainer.Surname}", BuildSetupLink(setupToken));
        return sent
            ? ServiceResult<object>.NoContent()
            : ServiceResult<object>.Error("email_send_failed", "Non è stato possibile inviare l'email di attivazione. Riprova più tardi.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int ptId)
    {
        var trainer = await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.PtId == ptId);
        if (trainer == null) return ServiceResult<object>.NotFound("trainer_not_found", "Personal trainer non trovato.");

        await using var transaction = await _slowFitContext.Database.BeginTransactionAsync();

        // Scollega clienti e piani, rimuovi gli appuntamenti del PT prima di eliminarlo.
        await _slowFitContext.Users
            .Where(u => u.PtId == ptId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(u => u.PtId, (int?)null));

        await _slowFitContext.Training
            .Where(t => t.PtId == ptId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.PtId, (int?)null));

        await _slowFitContext.Nutritions
            .Where(n => n.PtId == ptId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.PtId, (int?)null));

        await _slowFitContext.Appointments.Where(a => a.PtId == ptId).ExecuteDeleteAsync();
        await _slowFitContext.NotificationsFires.Where(n => n.ReceiverId == ptId && n.ReceiverRole == "trainer").ExecuteDeleteAsync();

        _slowFitContext.PersonalTrainers.Remove(trainer);
        await _slowFitContext.SaveChangesAsync();
        await transaction.CommitAsync();

        return ServiceResult<object>.NoContent();
    }

    // Genera un token monouso, salva sul PT solo l'hash + scadenza e restituisce il token in chiaro per l'email.
    private string GeneratePasswordSetupToken(PersonalTrainer trainer)
    {
        var rawToken = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        trainer.PasswordSetupTokenHash = HashToken(rawToken);
        trainer.PasswordSetupTokenExpiresAt = DateTime.UtcNow.Add(PasswordSetupTokenLifetime);
        return rawToken;
    }

    private string BuildSetupLink(string token)
    {
        var baseUrl = (_configuration["App:PasswordSetupUrl"] ?? "http://localhost:5051/slowFit/pt/activate").TrimEnd('/');
        var separator = baseUrl.Contains('?') ? '&' : '?';
        return $"{baseUrl}{separator}token={Uri.EscapeDataString(token)}";
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    private async Task<bool> IsEmailInUseAsync(string email, int? excludePtId)
    {
        var usedByUser = await _slowFitContext.Users.AnyAsync(u => u.Email == email);
        if (usedByUser) return true;

        return await _slowFitContext.PersonalTrainers
            .AnyAsync(p => p.Email == email && (excludePtId == null || p.PtId != excludePtId));
    }

    private static PersonalTrainerResponse ToResponse(PersonalTrainer pt, IReadOnlyList<int> clientIds) => new()
    {
        UserId = pt.PtId,
        FirstName = pt.FirstName,
        Surname = pt.Surname,
        Email = pt.Email,
        Address = pt.Address,
        City = pt.City,
        Country = pt.Country,
        Province = pt.Province,
        ZipCode = pt.ZipCode,
        BirthDate = pt.BirthDate,
        RoleId = PersonalTrainerRoleId,
        ImageProfile = pt.ImageProfile,
        Phone = pt.Phone,
        VatNumber = pt.VatNumber,
        FiscalCode = pt.FiscalCode,
        SdiCode = pt.SdiCode,
        PecEmail = pt.PecEmail,
        ClientIds = clientIds
    };
}
