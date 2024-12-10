using Application.Models;
using Application.Service;
using BlazorTemplate.Models;
using BlazorTemplateAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTemplateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccessControl<ApplicationUser> _accessControl;
        public AuthController(AccessControl<ApplicationUser> accessControl)
        {
            _accessControl = accessControl;
        }

        [HttpPost("Registration")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> Registration(RegistrationModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userReg = new ApplicationUser
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PasswordHash = user.Password
            };

            var result = await _accessControl.RegistrationAsync(userReg);
            if (result <= 0)
            {
                throw new Exception($"Регистрация не удалась");
            }
            return Created();
        }
        [HttpPost("Login")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> Login(LoginModel user)
        {
            var res = await _accessControl.LoginAsync(user.Email, user.Password);

            if (res == null)
            {
                throw new Exception($"Ошибка авторизации");
            }

            return Ok(new RequestAccess
            {
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken,
            });
        }
    }
}
