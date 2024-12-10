using Application.Models;
using BlazorTemplate.Models;
using BlazorTemplateAPI.Models.DTO;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BlazorTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET /user
        // Всегда возвращает список (может быть пустым)
        [HttpGet]
        public async Task<IEnumerable<UserDTO?>> Get()
        {

            var users = await _userManager.GetAllUsersAsync();
            var userDTO = new List<UserDTO>();
            foreach (var item in users)
            {
                var user = new UserDTO
                {
                    UserId = item.UserId,
                    UserName = item.UserName,
                    Email = item.Email,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Password = item.PasswordHash,
                    Roles = item.UserRoles.Select(ur => ur.Role).ToList()
                };
                userDTO.Add(user);
            }
            return userDTO;
        }
        // GET: api/user/[id]
        [HttpGet("{userId:Guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userManager.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден!");
            }
            var userDTO = new UserDTO
            {
                UserName = user.UserName,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Password = user.PasswordHash,
                Roles = user.UserRoles.Select(ur => ur.Role).ToList()
            };
            return Ok(userDTO);
        }
        // GET api/user/[email]
        [HttpGet("{email}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userManager.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден!");
            }
            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Password = user.PasswordHash,
                Roles = user.UserRoles.Select(ur => ur.Role).ToList()
            };
            return Ok(userDTO);
        }
        // POST: api/user
        // BODY: User (JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(User))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateUser([FromBody] RegistrationModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var rightUser = new ApplicationUser
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.Password
            };
            var result = await _userManager.CreateUserAsync(rightUser);
            return Ok();
        }
        // PUT api/user/[userId]
        // BODY: (JSON)
        [HttpPut("{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDTO user)
        {
            var t = "765636bc-b712-446e-c66b-08dd15c6c51a";
            Guid id = Guid.Parse(t);
            var existUser = await _userManager.GetUserByIdAsync(id);
            if (user.UserName != null) existUser.UserName = user.UserName;
            if (user.Email != null) existUser.Email = user.Email;
            _ = await _userManager.UpdateUserAsync(userId, existUser);
            return NoContent();
        }

        // POST api/user/[userId]/[duration]
        // Default 100 years
        [HttpPost("{userId:guid}/{duration}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteUserById(Guid userId, int duration = default)
        {
            if (duration == default || duration <= 0)
            {
                await _userManager.BlockUserByIdAsync(userId, default);
            }
            TimeSpan timeSpan = TimeSpan.FromHours(duration);
            var _ = await _userManager.BlockUserByIdAsync(userId, timeSpan);
            return NoContent();
        }

        // POST api/user/[userId]/[duration]
        [HttpPost("{email}/{duration:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> DeleteUserByEmail(string email, int duration = default)
        {
            if (duration == default || duration <= 0)
            {
                await _userManager.BlockUserByEmailAsync(email, default);
            }
            TimeSpan timeSpan = TimeSpan.FromHours(duration);
            var _ = await _userManager.BlockUserByEmailAsync(email, timeSpan);
            return NoContent();
        }
    }
}
