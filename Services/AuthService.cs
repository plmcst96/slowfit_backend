using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Services;

public sealed class AuthService(SlowFitContext slowFitContext, IConfiguration configuration) : IAuthService
{
    private readonly SlowFitContext _slowFitContext = slowFitContext;
    private readonly IConfiguration _configuration = configuration;
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);
    private readonly PasswordHasher<IAuthAccount> _passwordHasher = new();

    public async Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLogin request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<UserLoginResponse>.BadRequest("invalid_login", "Inserisci email e password.");
        }

        var account = await FindByEmailAsync(request.Email);
        if (account == null || !await IsValidPasswordAsync(account, request.Password))
        {
            return ServiceResult<UserLoginResponse>.Unauthorized("invalid_credentials", "Email o password non corretti.");
        }

        var refreshToken = IssueRefreshToken(account);
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<UserLoginResponse>.Ok(BuildLoginResponse(account, refreshToken, "Login successful!"));
    }

    public async Task<ServiceResult<UserLoginResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ServiceResult<UserLoginResponse>.BadRequest("invalid_refresh_token", "Refresh token non valido.");
        }

        var refreshTokenHash = HashRefreshToken(request.RefreshToken);
        var account = await FindByRefreshTokenHashAsync(refreshTokenHash);
        if (account == null)
        {
            return ServiceResult<UserLoginResponse>.Unauthorized("invalid_refresh_token", "Sessione scaduta o non valida. Effettua di nuovo il login.");
        }

        if (account.RefreshTokenRevokedAt.HasValue || !account.RefreshTokenExpiresAt.HasValue || account.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return ServiceResult<UserLoginResponse>.Unauthorized("expired_refresh_token", "Sessione scaduta. Effettua di nuovo il login.");
        }

        var refreshToken = IssueRefreshToken(account);
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<UserLoginResponse>.Ok(BuildLoginResponse(account, refreshToken, "Token refreshed."));
    }

    public async Task<ServiceResult<object>> LogoutAsync(RefreshTokenRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return ServiceResult<object>.BadRequest("invalid_refresh_token", "Refresh token non valido.");
        }

        var refreshTokenHash = HashRefreshToken(request.RefreshToken);
        var account = await FindByRefreshTokenHashAsync(refreshTokenHash);
        if (account == null)
        {
            return ServiceResult<object>.NoContent();
        }

        account.RefreshTokenRevokedAt = DateTime.UtcNow;
        await _slowFitContext.SaveChangesAsync();

        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<UserMeResponse>> GetMeAsync(int userId, int? roleId)
    {
        // Gli id possono coincidere tra le due tabelle: il roleId del token disambigua la sorgente.
        if (roleId == 2) // PersonalTrainerRoleId
        {
            var trainer = await _slowFitContext.PersonalTrainers
                .AsNoTracking()
                .Where(p => p.PtId == userId)
                .Select(p => new UserMeResponse
                {
                    UserId = p.PtId,
                    FirstName = p.FirstName,
                    Surname = p.Surname,
                    Email = p.Email,
                    Address = p.Address,
                    City = p.City,
                    Country = p.Country,
                    Province = p.Province,
                    ZipCode = p.ZipCode,
                    RoleId = 2,
                    BirthDate = p.BirthDate,
                    PtId = null,
                    ImageProfile = p.ImageProfile,
                    Phone = p.Phone
                })
                .FirstOrDefaultAsync();

            return trainer == null
                ? ServiceResult<UserMeResponse>.Unauthorized("invalid_token", "Utente non trovato. Effettua di nuovo il login.")
                : ServiceResult<UserMeResponse>.Ok(trainer);
        }

        var user = await _slowFitContext.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => new UserMeResponse
            {
                UserId = u.UserId,
                FirstName = u.FirstName,
                Surname = u.Surname,
                Email = u.Email,
                Address = u.Address,
                City = u.City,
                Country = u.Country,
                Province = u.Province,
                ZipCode = u.ZipCode,
                RoleId = u.RoleId,
                BirthDate = u.BirthDate,
                PtId = u.PtId,
                ImageProfile = u.ImageProfile,
                Phone = u.Phone
            })
            .FirstOrDefaultAsync();

        return user == null
            ? ServiceResult<UserMeResponse>.Unauthorized("invalid_token", "Utente non trovato. Effettua di nuovo il login.")
            : ServiceResult<UserMeResponse>.Ok(user);
    }

    private async Task<IAuthAccount?> FindByEmailAsync(string email)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user != null) return user;
        return await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.Email == email);
    }

    private async Task<IAuthAccount?> FindByRefreshTokenHashAsync(string refreshTokenHash)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.RefreshTokenHash == refreshTokenHash);
        if (user != null) return user;
        return await _slowFitContext.PersonalTrainers.FirstOrDefaultAsync(p => p.RefreshTokenHash == refreshTokenHash);
    }

    private string IssueRefreshToken(IAuthAccount account)
    {
        var refreshToken = GenerateRefreshToken();
        account.RefreshTokenHash = HashRefreshToken(refreshToken);
        account.RefreshTokenExpiresAt = DateTime.UtcNow.Add(RefreshTokenLifetime);
        account.RefreshTokenRevokedAt = null;
        return refreshToken;
    }

    private UserLoginResponse BuildLoginResponse(IAuthAccount account, string refreshToken, string message) => new()
    {
        Email = account.AccountEmail,
        Message = message,
        UserId = account.AccountId,
        RoleId = account.AccountRoleId,
        Token = GenerateToken(account.AccountId, account.AccountEmail, account.AccountRoleId),
        RefreshToken = refreshToken,
        RefreshTokenExpiresAt = account.RefreshTokenExpiresAt
    };

    private string GenerateToken(int userId, string email, int? roleId)
    {
        var jwtKey = _configuration["JwtConfiguration:Key"] ?? throw new InvalidOperationException("JwtConfiguration:Key is required.");
        var jwtIssuer = _configuration["JwtConfiguration:Issuer"];
        var jwtAudience = _configuration["JwtConfiguration:Audience"];
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new("userId", userId.ToString())
        };

        if (roleId.HasValue)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleId.Value.ToString()));
            claims.Add(new Claim("roleId", roleId.Value.ToString()));
        }

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private static string HashRefreshToken(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToBase64String(hash);
    }

    private async Task<bool> IsValidPasswordAsync(IAuthAccount account, string password)
    {
        var stored = account.AccountPassword;

        // PT non ancora attivato: nessuna password impostata -> login non consentito.
        if (string.IsNullOrEmpty(stored))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(account, stored, password);
        if (result != PasswordVerificationResult.Failed)
        {
            return true;
        }

        if (stored != password)
        {
            return false;
        }

        account.AccountPassword = _passwordHasher.HashPassword(account, password);
        await _slowFitContext.SaveChangesAsync();
        return true;
    }
}
