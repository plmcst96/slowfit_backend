using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Controllers
{
    [Route("slowFit/login")]
    [ApiController]
    public class UserLoginController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;
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

                if (user == null || user.Password != userLogin.Password) // Consider hashing per sicurezza
                {
                    return Unauthorized("Invalid email or password ");
                }

                var response = new UserLoginResponse
                {
                    Email = user.Email,
                    Message = "Login successful!",
                    UserId = user.UserId,
                    RoleId = user.RoleId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest("An error occurred during login.");
            }
        }
    }
}
