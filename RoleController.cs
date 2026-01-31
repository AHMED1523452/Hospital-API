using Hospital_API.DTO;
using Hospital_API.Model;
using Hospital_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Hospital_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class RoleController : ControllerBase
    {
        public RoleController(IRoleRepository roleRepository )
        {
            RoleRepository = roleRepository;
        }

        public IRoleRepository RoleRepository { get; }

        [HttpGet("Roles")]
        [ProducesResponseType(typeof(List<GettingRolesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<GettingRolesDTO>>> GetRoles()
        {
            var roles = await RoleRepository.GetRoles();
            return Ok(roles);
        }

        [HttpPost("CreateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (ModelState.IsValid)
            {
                if(await RoleRepository.GetRoleByName(roleName) == null)
                {
                    await RoleRepository.CreateRole(roleName);
                    return Ok("Role created successfully");
                }
                return BadRequest("Role already exist!!");
            }
            return BadRequest("Invalid input!");
        }

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromQuery]string Oldname,RoleDTO roleDTO)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleRepository.GetRoleByName(Oldname);

                if (role == null) 
                {
                    return BadRequest("Role does not exist");
                }

                role.Name = roleDTO.NewRoleName;
                role.NormalizedName = roleDTO.NewRoleName.ToUpper();
                await RoleRepository.Update(role);
                return Ok("Role updated successfully");
            }
            return BadRequest("Invalid input!");
        }

        [HttpDelete("DeleteRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteRole(string Name)
        {
            if (ModelState.IsValid)
            {
                var role = await RoleRepository.GetRoleByName(Name);
                if (role == null)
                    return NotFound("Invalid role name");
                await RoleRepository.DeleteRole(role);
                return Ok("Role Deleted Successfully!");
            }
            return BadRequest("Send valid inputs");
        }

        [HttpPost("Users")]
        [ProducesResponseType(typeof(List<UsersInRoleDTO>) , StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<UsersInRoleDTO>>> GetUsersInRole([FromQuery] string RoleName)
        {
            if (ModelState.IsValid)
            {
                return Ok(await RoleRepository.GetUsersInRoleAsync(RoleName));
            }
            return BadRequest();
        }
    }
}
