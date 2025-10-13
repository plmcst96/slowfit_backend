using System;
using Microsoft.AspNetCore.Mvc;
using slowfit.DBModels;
using slowfit.DTORequest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/role")]
    [ApiController]
    public class RoleUserController(SlowFitContext slowFitCtx) : ControllerBase
    {
        private readonly SlowFitContext _slowFitContext = slowFitCtx;


        [HttpGet]
        public ActionResult<IEnumerable<RoleUserRes>> GetAll()
        {
            var roleList = new List<RoleUserRes>();
            try
            {

                roleList = _slowFitContext.RoleUsers.Select(r => new RoleUserRes
                {
                    RoleId = r.RoleId,
                    RoleName = r.RoleName,

                }).ToList();

                if (roleList.Count == 0) return NoContent();

                return Ok(roleList);
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] RoleUserRes roleUser)
        {
            if (roleUser == null)
            {
                return BadRequest("Data role incorrect");
            }

            if (string.IsNullOrEmpty(roleUser.RoleName) || roleUser.RoleId == 0)
            {
                return BadRequest();
            }
            try
            {
                var role = new RoleUser()
                {
                    RoleName = roleUser.RoleName,
                };
                _slowFitContext.RoleUsers.Add(role);
                _slowFitContext.SaveChanges();
                return Ok("Role created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create role");
            }
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] RoleUserRes roleUser)
        {
            if (roleUser == null || roleUser.RoleId != id)
            {
                return BadRequest("Invalid data or ID does not match");
            }

            var existingRole = _slowFitContext.RoleUsers.Where(r => r.RoleId == id).FirstOrDefault();
            if (existingRole == null)
            {
                return NotFound("Plan not found");
            }

            // Aggiorna i dati
            existingRole.RoleName = roleUser.RoleName;


            try
            {
                _slowFitContext.RoleUsers.Update(existingRole);


                _slowFitContext.SaveChanges();

                return Ok("Role updated succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to update role: {roleUser.RoleName}");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _slowFitContext.RoleUsers.Where(r => r.RoleId == id).FirstOrDefault();

            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            try
            {
                _slowFitContext.RoleUsers.Remove(role);
                _slowFitContext.SaveChanges();
                return Ok("Role delete succesfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error to delete role {role.RoleName} ");
            }
        }
    }
}
