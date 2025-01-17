using Application.Models;
using AutoMapper;
using BlazorTemplate.Models;
using BlazorTemplateAPI.Models;
using BlazorTemplateAPI.Models.DTO;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BlazorTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET /user
        // Всегда возвращает список (может быть пустым)
        [HttpGet]
        public async Task<ActionResult<PagginatedModel<UserDTO>>> Get(int page, int pageSize)
        {

            var (users, lastPage) = await _userManager.GetAllUsersAsync(page, pageSize);
            var userDTO = new List<UserDTO>();
            foreach (var item in users)
            {

                //var user = _mapper.Map<UserDTO>(item);

                var user = new UserDTO
                {
                    UserId = item.UserId,
                    UserName = item.UserName,
                    Email = item.Email,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Password = item.PasswordHash,
                    CreatedAt = item.CreatedAt,
                    LastUpdateAt = item.LastUpdateAt,
                    isLocked = item.IsLocked,
                    BlockedUntil = item.BlockedUntil,
                    Roles = item.UserRoles.Select(ur => ur.Role).ToList()
                };
                userDTO.Add(user);
            }

            var response = new PagginatedModel<UserDTO>
            {
                Items = userDTO,
                LastPage = lastPage,
            };
            return Ok(response);
        }
        [HttpGet("role")]
        public async Task<ActionResult<PagginatedModel<UserDTO>>> Get(string roles, int page, int pageSize)
        {

            var rolesArray = roles.Split(',');

            var (users, lastPage) = await _userManager.GetUsersByRole(page, pageSize, rolesArray);
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
                    CreatedAt = item.CreatedAt,
                    LastUpdateAt = item.LastUpdateAt,
                    isLocked = item.IsLocked,
                    BlockedUntil = item.BlockedUntil,
                    Roles = item.UserRoles.Select(ur => ur.Role).ToList()
                };
                userDTO.Add(user);
            }

            var response = new PagginatedModel<UserDTO>
            {
                Items = userDTO,
                LastPage = lastPage,
            };
            return Ok(response);
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
        public async Task<IActionResult> CreateUser([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map<ApplicationUser>(model);

            var result = await _userManager.CreateUserAsync(user);
            return Ok();
        }
        // PUT api/user/[userId]
        // BODY: (JSON)
        [HttpPut("Update/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUser updateUser)
        {

            var user = _mapper.Map<ApplicationUser>(updateUser);


            var result = await _userManager.UpdateUserAsync(userId, user);
            if (result <= 0)
            {
                return BadRequest();
            }
            return NoContent();
        }

        // POST api/user/[userId]/[duration]
        // Default 100 years
        [HttpPost("Block/{userId:guid}/{duration}")]
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

        // POST api/user/[userId]/[duration] UnblockUserByIdAsync
        [HttpPost("Block/{email}/{duration:int}")]
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
        // POST api/user/[userId]/[duration] UnblockUserByIdAsync
        [HttpPost("UnBlock/{userId:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UnblockUserById(Guid userId)
        {
            var result = await _userManager.UnblockUserByIdAsync(userId);
            if(result <= 0)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
