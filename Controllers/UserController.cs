using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.DTOResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/user")]
    [ApiController]
    public class UserController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;


        [HttpGet]
        public IActionResult GetUsersByRole([FromQuery] int roleId)
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            try
            {
                var role = _slowFitContext.RoleUsers.Where(r => r.RoleId == roleId).FirstOrDefault();

                if (role == null)
                {
                    return NotFound(new { message = "Role not Found" });
                }

                var usersByRole = _slowFitContext.Users
                    .Where(u => u.RoleId == roleId)
                    .Select(u => new UserRes
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Surname = u.Surname,
                        RoleId = u.RoleId,
                        PtId = u.PtId,
                        Phone = u.Phone,
                        ImageProfile = u.ImageProfile
                    })
                    .ToList();

                return Ok(usersByRole);
            }
            catch (Exception)
            {
                return BadRequest("No users found whit this role");
            }
        }

        [HttpGet("pt/{ptId}")]
        public ActionResult<IEnumerable<UserRes>> GetUsersByPtId(int ptId)
        {
            if (!User.IsPersonalTrainer() && User.GetUserId() != ptId) return Forbid();

            try
            {
                // Fetch users by ptId directly and map them to UserRes
                var userList = _slowFitContext.Users
                    .Where(u => u.PtId == ptId)
                    .Select(u => new UserRes
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        Surname = u.Surname,
                        RoleId = u.RoleId,
                        PtId = u.PtId,
                        Phone = u.Phone,
                      ImageProfile = u.ImageProfile
                    })
                    .ToList();

                // If no users were found, return a NotFound response
                if (userList == null || userList.Count == 0)
                {
                    return NotFound(new { message = "No users found for the given ptId" });
                }

                // Return the list of users
                return Ok(userList);
            }
            catch (Exception ex)
            {
                // Return a BadRequest response with the exception message
                return BadRequest(new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }




        [HttpGet("alluser")]
        public ActionResult<IEnumerable<UserProfile>> GetAllUsers()
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            var userList = new List<UserProfile>();
            try
             {

                userList = _slowFitContext.Users.Select(u => new UserProfile
                {
                   UserId = u.UserId,
                   FirstName = u.FirstName,
                   Surname = u.Surname,
                   Email = u.Email,
                   Address = u.Address!,
                   City = u.City!,
                   Country = u.Country!,
                   Province = u.Province!,
                   ZipCode = u.ZipCode!,
                   BirthDate = u.BirthDate,
                   RoleId = u.RoleId,
                   ImageProfile = u.ImageProfile!,
                   Phone = u.Phone!

                }).ToList();

                if (userList.Count == 0) return NoContent();

                return Ok(userList);
            }
            catch (Exception)
            {
                return BadRequest( $"An error occurred");
            }

        }


        [HttpGet("{id}")]
        public ActionResult<UserRes> GetProfile(int id)
        {
            if (!User.CanAccessUser(id)) return Forbid();

            try
            {
                var profile = _slowFitContext.Users.Where(u => u.UserId == id).Select(u => new UserRes
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    Surname = u.Surname,
                    Email = u.Email,
                    RoleId = u.RoleId!,
                    PtId = u.PtId,
                    Phone = u.Phone,
                   
                }).FirstOrDefault();

                if (profile == null)
                {
                    return NotFound(new { message = "Profilo non trovato" });
                }

                return Ok(profile);
            }
            catch (Exception)
            {
                return BadRequest("Error to loading profile");
            }
        }

        [HttpGet("byEmail/{email}")]
        public ActionResult<UserProfile> GetProfileByEmail(string email)
        {
            try
            {
                var profile = _slowFitContext.Users
                    .Where(u => u.Email == email)
                    .Select(u => new UserProfile
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        Surname = u.Surname,
                        Email = u.Email,
                        Address = u.Address ?? null,
                        City = u.City ?? null,
                        Country = u.Country ?? null,
                        Province = u.Province ?? null,
                        ZipCode = u.ZipCode ?? null,
                        BirthDate = u.BirthDate,
                        RoleId = u.RoleId!,
                        ImageProfile = u.ImageProfile ?? null,
                        Phone = u.Phone ?? null

                    })
                    .FirstOrDefault();

                if (profile == null)
                {
                    return NotFound(new { message = "Profile not found" });
                }

                if (!User.CanAccessUser(profile.UserId)) return Forbid();

                return Ok(profile);
            }
            catch (Exception)
            {
                return BadRequest("Error loading profile");
            }
        }


        [HttpPost("profile/{userId}")]
        public IActionResult CreateProfile(int userId, [FromBody] AddProfile newUser)
        {
            if (!User.CanAccessUser(userId)) return Forbid();

            if (newUser == null)
            {
                return BadRequest("Invalid request body.");
            }

            if (string.IsNullOrEmpty(newUser.Address) ||
                string.IsNullOrEmpty(newUser.City) ||
                string.IsNullOrEmpty(newUser.Country) ||
                string.IsNullOrEmpty(newUser.Province) ||
                newUser.ZipCode == 0 ||
                newUser.BirthDate == null)
            {
                return BadRequest("Missing required fields.");
            }

            try
            {
                var existingUser = _slowFitContext.Users.FirstOrDefault(u => u.UserId == userId);
                if (existingUser == null)
                {
                    return NotFound($"User with id {userId} not found.");
                }

                existingUser.Address = newUser.Address;
                existingUser.City = newUser.City;
                existingUser.Province = newUser.Province;
                existingUser.Country = newUser.Country;
                existingUser.ZipCode = newUser.ZipCode;
                existingUser.ImageProfile = newUser.ImageProfile;
                existingUser.BirthDate = newUser.BirthDate?.Date;
                existingUser.Phone = newUser.Phone;

                _slowFitContext.SaveChanges();
                return Ok("Profile updated successfully.");
            }
            catch (Exception)
            {
                return BadRequest("Failed to update profile information.");
            }
        }



        // PUT: slowFit/alluser/{id}
        [HttpPut("profile/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserProfile updatedUser)
        {
            if (!User.CanAccessUser(id)) return Forbid();

            if (updatedUser == null || updatedUser.UserId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingUser = _slowFitContext.Users.Where(u => u.UserId == id).FirstOrDefault();
            if (existingUser == null)
            {
                return NotFound("User not found");
            }

            // Aggiorna i dati
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.Surname = updatedUser.Surname;
            existingUser.BirthDate = updatedUser.BirthDate?.Date;
            existingUser.Province = updatedUser.Province;
            existingUser.Country = updatedUser.Country;
            existingUser.City = updatedUser.City;
            existingUser.ZipCode = updatedUser.ZipCode;
            existingUser.Address = updatedUser.Address;
            existingUser.ImageProfile = updatedUser.ImageProfile;
            existingUser.Phone = updatedUser.Phone;

            try
            {
                _slowFitContext.Users.Update(existingUser);


                _slowFitContext.SaveChanges();

                return Ok("User updated succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Failed to update profile of {updatedUser.FirstName} {updatedUser.Surname}");
            }
        }

        // DELETE: slowFit/alluser/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!User.IsPersonalTrainer()) return Forbid();

            var profile = _slowFitContext.Users.Where(u => u.UserId == id).FirstOrDefault();

            if (profile == null)
            {
                return BadRequest($"No user found whit this {id}");
            }

            try
            {
                _slowFitContext.Users.Remove(profile);
                _slowFitContext.SaveChanges();
                return Ok("User deleted succesfully");
            }
            catch (Exception)
            {
                return BadRequest($"Error to delete user {profile.FirstName} {profile.Surname}");
            }
            
        }
    }
}
