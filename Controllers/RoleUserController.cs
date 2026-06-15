using Microsoft.AspNetCore.Mvc;
using slowfit.Auth;
using slowfit.DTORequest;
using slowfit.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace slowfit.Controllers
{
    [Route("slowFit/role")]
    [ApiController]
    public class RoleUserController(IRoleService roleService) : ControllerBase
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return this.ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RoleUpsertRequest roleUser)
        {
            if (!User.IsSuperAdmin()) return Forbid();

            var result = await _roleService.CreateAsync(roleUser);
            return this.ToActionResult(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RoleUpsertRequest roleUser)
        {
            if (!User.IsSuperAdmin()) return Forbid();

            var result = await _roleService.UpdateAsync(id, roleUser);
            return this.ToActionResult(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!User.IsSuperAdmin()) return Forbid();

            var result = await _roleService.DeleteAsync(id);
            return this.ToActionResult(result);
        }
    }
}
