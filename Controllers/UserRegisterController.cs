using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

namespace slowfit.Controllers
{
    [Route("slowFit/register")]
    [ApiController]
    [AllowAnonymous]
    public class UserRegisterController : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext;
        private readonly PasswordHasher<User> _passwordHasher = new();

        // Costruttore pubblico con DI
        public UserRegisterController(SlowFitContext slowFitCtx)
        {
            _slowFitContext = slowFitCtx;
        }
        [HttpPost]
        public ActionResult<UserRegisterResponse> Register([FromBody] UserRegister userRegister)
        {
            if (userRegister == null ||
                string.IsNullOrEmpty(userRegister.FirstName) ||
                string.IsNullOrEmpty(userRegister.Email) ||
                string.IsNullOrEmpty(userRegister.Password) ||
                userRegister.RoleId <= 0)
            {
                return BadRequest(new UserRegisterResponse { Message = "All fields are required." });
            }

            try
            {
                if (_slowFitContext.Users.Any(u => u.Email == userRegister.Email))
                {
                    return BadRequest(new UserRegisterResponse { Message = "Email is already in use." });
                }

                var user = new User
                {
                    FirstName = userRegister.FirstName,
                    Surname = userRegister.Surname,
                    Email = userRegister.Email,
                    RoleId = userRegister.RoleId,
                    PtId = userRegister.PtId // 👈 può essere null
                };

                user.Password = _passwordHasher.HashPassword(user, userRegister.Password);

                _slowFitContext.Users.Add(user);
                _slowFitContext.SaveChanges();

                return Ok(new UserRegisterResponse
                {
                    UserId = user.UserId,
                    RoleId = (int)user.RoleId,
                    Message = "New user has been added successfully!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new UserRegisterResponse { Message = "Error registering new user: " + ex.Message });
            }
        }
    }
}
