using Application.Interfaces;
using Application.Models;
using Application.Models.AuthModels;
using Application.Models.DTO;
using AutoMapper;
using BlazorTemplateAPI.Models;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace BlazorTemplate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // GET /user
        // ¬сегда возвращает список (может быть пустым)
        [HttpGet]
        public async Task<ActionResult<PagginationModel<UserDTO>>> Get(int page, int offset, string? searchQuery)
        {
            if (page <= 0 || offset <= 0)
            {
                return BadRequest("—траница или размер страницы не может быть отрицательна или равно нулю");
            }
            var (users, totalPage) = await _userService.GetAllUsersAsync(page, offset, searchQuery);

            var result = new PagginationModel<UserDTO>
            {
                Items = users,
                LatPage = totalPage
            };
            return Ok(result);
        }
        // GET /user/byroles
        // ¬сегда возвращает список (может быть пустым)
        [HttpGet("byroles")]
        public async Task<ActionResult<PagginationModel<UserDTO>>> Get(int page, int offset, [FromQuery]string[] roles, string? searchQuery)
        {
            if (page <= 0 || offset <= 0)
            {
                return BadRequest("—траница или размер страницы не может быть отрицательна или равно нулю");
            }
            if (!roles.Any())
            {
                return BadRequest("—писок ролей не может быть пуст.");
            }
            var (users, totalPage) = await _userService.GetUsersByAllRolesAsync(page, offset,roles, searchQuery);

            var result = new PagginationModel<UserDTO>
            {
                Items = users,
                LatPage = totalPage
            };
            return Ok(result);
        }
        //GET: api/user/[id]
        [HttpGet("{userId:Guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound("ѕользователь не найден.");
            }
            return Ok(user);
            
        }
        // GET api/user/[email]
        [HttpGet("{email}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound("ѕользователь не найден.");
            }
            return Ok(user);
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

           var user = await _userService.CreateUserAsync(model);

            return CreatedAtAction(nameof(CreateUser), user);
        }
        // PUT api/user/[userId]
        // BODY: (JSON)
        [HttpPost("Update/{userId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserDTO updateUser)
        {

            var result = await _userService.UpdateUserAsync(userId, updateUser);
            if (result == null)
            {
                return BadRequest();
            }
            return NoContent();
        }
        [HttpPost("block")]
        public async Task<IActionResult> BlockUser(Guid userId, DateTime duration)
        {

            await _userService.BlockUserAsync(userId, duration);
            return Ok();
        }

        // –азблокировка пользовател€
        [HttpPost("unblock")]
        public async Task<IActionResult> UnblockUser(Guid userId)
        {
            try
            {
                await _userService.UnblockUserAsync(userId);
                return Ok($"User {userId} unblocked successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error unblocking user: {ex.Message}");
            }
        }
    }
}
