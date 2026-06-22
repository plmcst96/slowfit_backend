using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace slowfit.DBModels;

/// <summary>
/// Astrazione condivisa tra le entità che possono autenticarsi (User e PersonalTrainer),
/// così l'AuthService gestisce login/refresh/logout con un'unica code-path.
/// </summary>
public interface IAuthAccount
{
    int AccountId { get; }

    string AccountEmail { get; }

    string? AccountPassword { get; set; }

    int AccountRoleId { get; }

    string? RefreshTokenHash { get; set; }

    DateTime? RefreshTokenExpiresAt { get; set; }

    DateTime? RefreshTokenRevokedAt { get; set; }
}

public partial class User : IAuthAccount
{
    [NotMapped]
    public int AccountId => UserId;
    [NotMapped]
    public string AccountEmail => Email;
    [NotMapped]
    public string? AccountPassword { get => Password; set => Password = value ?? string.Empty; }
    [NotMapped]
    public int AccountRoleId => RoleId ?? 1; // UserRoleId
}

public partial class PersonalTrainer : IAuthAccount
{
    [NotMapped]
    public int AccountId => PtId;
    [NotMapped]
    public string AccountEmail => Email;
    [NotMapped]
    public string? AccountPassword { get => Password; set => Password = value; }
    [NotMapped]
    public int AccountRoleId => 2; // PersonalTrainerRoleId
}
