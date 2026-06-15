using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace slowfit.Controllers
{
    [Route("slowFit/login")]
    [ApiController]
    [AllowAnonymous]
    public class UserLoginController(SlowFitContext slowFitCtx, IConfiguration configuration) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;
        private readonly IConfiguration _configuration = configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        [HttpPost]
        public ActionResult<UserLoginResponse> Login([FromBody] UserLogin userLogin)
        {
            if (userLogin == null ||
                string.IsNullOrEmpty(userLogin.Email) ||
                string.IsNullOrEmpty(userLogin.Password))  // RoleId deve essere specificato
            {
                return BadRequest("Email and Password are required.");
            }

            try
            {
                var user = _slowFitContext.Users
                    .Where(u => u.Email == userLogin.Email)
                    .FirstOrDefault();

                if (user == null || !IsValidPassword(user, userLogin.Password))
                {
                    return Unauthorized("Invalid email or password ");
                }

                var response = new UserLoginResponse
                {
                    Email = user.Email,
                    Message = "Login successful!",
                    UserId = user.UserId,
                    RoleId = user.RoleId,
                    Token = GenerateToken(user.UserId, user.Email, user.RoleId)
                };

                return Ok(response);
            }
            catch (Exception)
            {
                return BadRequest("An error occurred during login.");
            }
        }

        private string GenerateToken(int userId, string email, int? roleId)
        {
            var jwtKey = _configuration["JwtConfiguration:Key"];
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
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool IsValidPassword(User user, string password)
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
            _slowFitContext.SaveChanges();
            return true;
        }
    }
}
