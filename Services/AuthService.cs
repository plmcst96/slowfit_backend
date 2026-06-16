using System.IdentityModel.Tokens.Jwt;
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
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<ServiceResult<UserLoginResponse>> LoginAsync(UserLogin request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return ServiceResult<UserLoginResponse>.BadRequest("invalid_login", "Inserisci email e password.");
        }

        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !await IsValidPasswordAsync(user, request.Password))
        {
            return ServiceResult<UserLoginResponse>.Unauthorized("invalid_credentials", "Email o password non corretti.");
        }

        return ServiceResult<UserLoginResponse>.Ok(new UserLoginResponse
        {
            Email = user.Email,
            Message = "Login successful!",
            UserId = user.UserId,
            RoleId = user.RoleId,
            Token = GenerateToken(user.UserId, user.Email, user.RoleId)
        });
    }

    public async Task<ServiceResult<UserLoginResponse>> RefreshAsync(int userId)
    {
        var user = await _slowFitContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return ServiceResult<UserLoginResponse>.Unauthorized("invalid_token", "Utente non trovato. Effettua di nuovo il login.");
        }

        return ServiceResult<UserLoginResponse>.Ok(new UserLoginResponse
        {
            Email = user.Email,
            Message = "Token refreshed.",
            UserId = user.UserId,
            RoleId = user.RoleId,
            Token = GenerateToken(user.UserId, user.Email, user.RoleId)
        });
    }

    public async Task<ServiceResult<UserMeResponse>> GetMeAsync(int userId)
    {
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

    private async Task<bool> IsValidPasswordAsync(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
        if (result != PasswordVerificationResult.Failed)
        {
            return true;
        }

        if (user.Password != password)
        {
            return false;
        }

        user.Password = _passwordHasher.HashPassword(user, password);
        await _slowFitContext.SaveChangesAsync();
        return true;
    }
}
