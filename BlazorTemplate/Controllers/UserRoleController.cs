using Application.Interfaces;
using Application.Models.DTO;
using BlazorTemplateAPI.Models;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTemplate.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }
        // GET /UserRole
        // Всегда возвращает список (может быть пустым)
        [HttpGet]
        public async Task<IEnumerable<RoleDTO?>> Get()
        {
            var users = await _userRoleService.GetAllRolesAsync();
            return users;
        }
        // GET: api/UserRole/[id]
        [HttpGet("{userId:Guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get(Guid userId)
        {
            var roles = await _userRoleService.GetUserRolesByIdAsync(userId);
            if (roles == null)
            {
                return NotFound();
            }
            return Ok(roles);
        }


        [HttpPost("AddRange/{userId:guid}")]
        public async Task<IActionResult> AssygnRoleRange(Guid userId, [FromBody] Dictionary<string, bool> roles)
        {
            var result = await _userRoleService.AssingRangeRolesAsync(userId, roles);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}