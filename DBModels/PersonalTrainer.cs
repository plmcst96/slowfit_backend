using System;
using System.Collections.Generic;

namespace slowfit.DBModels;

public partial class PersonalTrainer
{
    public int PtId { get; set; }

    public string FirstName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    // Nullable: alla creazione il PT non ha password; verrà impostata col flusso email.
    public string? Password { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Province { get; set; }

    public int? ZipCode { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? ImageProfile { get; set; }

    public string? Phone { get; set; }

    public string? FcmToken { get; set; }

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiresAt { get; set; }

    public DateTime? RefreshTokenRevokedAt { get; set; }

    // Token monouso per l'impostazione della password via email (creazione/reset da parte del super admin).
    public string? PasswordSetupTokenHash { get; set; }

    public DateTime? PasswordSetupTokenExpiresAt { get; set; }

    // Dati fiscali
    public string? VatNumber { get; set; }

    public string? FiscalCode { get; set; }

    public string? SdiCode { get; set; }

    public string? PecEmail { get; set; }

    public virtual ICollection<User> Clients { get; set; } = new List<User>();

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Training> Trainings { get; set; } = new List<Training>();

    public virtual ICollection<Nutrition> Nutritions { get; set; } = new List<Nutrition>();
}
