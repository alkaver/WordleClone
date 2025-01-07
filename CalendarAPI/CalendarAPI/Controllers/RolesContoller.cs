using CalendarAPI.Dtos;
using CalendarAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CalendarAPI.Controllers
{
    // [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RolesContoller : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly UserManager<User> _userManager;

        public RolesContoller(RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("createRole")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            if (string.IsNullOrEmpty(createRoleDto.RoleName))
            {
                return BadRequest("Role Name is required");
            }

            var roleExist = await _roleManager.RoleExistsAsync(createRoleDto.RoleName);

            if (roleExist)
            {
                return BadRequest("Role already exists");
            }

            var roleResult = await _roleManager.CreateAsync(new IdentityRole(createRoleDto.RoleName));

            if (roleResult.Succeeded)
            {
                return Ok(new { message = "Role Created successfully" });
            }

            return BadRequest("Role creation failed");
        }

        [HttpGet("getRoles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            var roleResponses = new List<RoleResponseDto>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!); 

                roleResponses.Add(new RoleResponseDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    TotalUsers = usersInRole.Count 
                });
            }

            return Ok(roleResponses);
        }


        [HttpDelete("deleteRole")]
        public async Task<IActionResult> DeleteRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role Name is required");
            }

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound($"Role '{roleName}' not found");
            }

            var roleResult = await _roleManager.DeleteAsync(role);

            if (roleResult.Succeeded)
            {
                return Ok(new { message = "Role deleted successfully" });
            }

            return BadRequest("Failed to delete role");
        }


        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRole([FromBody] RoleAssignDto roleAssignDto)
        {
            var user = await _userManager.FindByIdAsync(roleAssignDto.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleAssignDto.RoleName);
            if (!roleExists)
            {
                return BadRequest(new { message = "Role does not exist" });
            }

            var result = await _userManager.AddToRoleAsync(user, roleAssignDto.RoleName);

            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            return BadRequest(new { message = "Error assigning role", errors = result.Errors });
        }


    }
}
