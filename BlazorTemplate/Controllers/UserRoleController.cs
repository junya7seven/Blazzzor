using BlazorTemplate.Models;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTemplate.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly RoleManager<ApplicationUser> _roleManager;

        public UserRoleController(RoleManager<ApplicationUser> roleManager)
        {
            _roleManager = roleManager;
        }
        // GET /UserRole
        // Всегда возвращает список (может быть пустым)
        [HttpGet]
        public async Task<IEnumerable<Role?>> Get()
        {
            var users = await _roleManager.GetAllRolesAsync();
            return users;
        }
        // GET: api/UserRole/[id]
        [HttpGet("{userId:Guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid userId)
        {
            var user = await _roleManager.GetUserRolesByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        // GET: api/UserRole/[email]
        [HttpGet("{email}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(string email)
        {
            var user = await _roleManager.GetUserRolesByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/UserRole/Users/[roleName]
        [HttpGet("Users/{roleName}")]
        [ProducesResponseType(200, Type = typeof(User))]
        public async Task<IEnumerable<ApplicationUser>> GetUsersByRole(string roleName)
        {
            var users = await _roleManager.GetUsersByRoleAsync(roleName);
            return users;
        }
        // POST: api/UserRole/Users/[userId]/[roleName]
        [HttpPost("Users/{userId:Guid}/{roleName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AssignRoleById(Guid userId, string roleName)
        {
            var result = await _roleManager.AssignRoleByIdAsync(userId, roleName);
            if(result == 1)
            {
                return Ok();
            }
            return BadRequest();
        }
        // POST: api/UserRole/Users/[email]/[roleName]
        [HttpPost("Users/{email}/{roleName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AssignRoleByEmail(string email, string roleName)
        {
            var result = await _roleManager.AssignRoleByEmailAsync(email, roleName);
            if (result == 1)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("AddRange/{userId:guid}")]
        public async Task<IActionResult> AssygnRoleRange(Guid userId, [FromBody] Dictionary<string, bool> roles)
        {
            var res = await _roleManager.AssingRangeRolesAsync(userId, roles);
            if (res)
            {
                return Ok();

            }
            return BadRequest();
        }

            // POST: api/UserRole/Users/Revoke/[userId]/[roleName]
        [HttpPost("Users/Revoke/{userId:Guid}/{roleName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RevokeRole(Guid userId, string roleName)
        {
            var result = await _roleManager.RevokeRoleAsync(userId, roleName);
            if(result == 5)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost("{roleName}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var result = await _roleManager.CreateRoleAsync(roleName);
            if(result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
