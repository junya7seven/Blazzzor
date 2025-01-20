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

        // GET /users
        [HttpGet]
        public async Task<ActionResult<PagginatedModel<UserDTO>>> Get(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Страница или размер страницы не может быть отрицательной или равна нулю");
            }
            var (users, totalPage) = await _userService.GetAllUsersAsync(page, pageSize);

            var result = new PagginatedModel<UserDTO>
            {
                Items = users,
                LastPage = totalPage
            };
            return Ok(result);
        }
        // GET /user/byroles
        [HttpGet("byroles")]
        public async Task<ActionResult<PagginatedModel<UserDTO>>> Get(int page, int pageSize, [FromQuery] string[] roles)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Страница или размер страницы не может быть отрицательной или равна нулю");
            }
            if (!roles.Any())
            {
                return BadRequest("Список ролей пуст, используйте другой запрос.");
            }
            var (users, totalPage) = await _userService.GetUsersByAllRolesAsync(page, pageSize, roles);

            var result = new PagginatedModel<UserDTO>
            {
                Items = users,
                LastPage = totalPage
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
                return NotFound("Пользователь не найден.");
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
                return NotFound("Пользователь не найден.");
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
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDTO updateUser)
        {

            var result = await _userService.UpdateUserAsync(userId, updateUser);
            if (result == null)
            {
                throw new ArgumentNullException("Пользователь для обновления не может быть пуст.");
            }
            return NoContent();
        }
        [HttpPost("block/{userId:guid}/{duration:datetime}")]
        public async Task<IActionResult> BlockUser(Guid userId, DateTime duration)
        {
            await _userService.BlockUserAsync(userId, duration);
            return Ok();
        }

        [HttpPost("unblock/{userId:guid}")]
        public async Task<IActionResult> UnblockUser(Guid userId)
        {
            await _userService.UnblockUserAsync(userId);
            return Ok();
        }
    }
}