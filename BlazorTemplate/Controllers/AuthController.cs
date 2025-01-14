using Application;
using Application.Models;
using Application.Service;
using AutoMapper;
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
        private readonly JwtSettings _jwtSettings;
        private readonly IMapper _mapper;
        public AuthController(AccessControl<ApplicationUser> accessControl, JwtSettings jwtSettings, IMapper mapper)
        {
            _accessControl = accessControl;
            _jwtSettings = jwtSettings;
            _mapper = mapper;
        }

        [HttpPost("Registration")]
        [ProducesResponseType(201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> Registration(RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _mapper.Map<ApplicationUser>(model);


            var result = await _accessControl.RegistrationAsync(user);
            if (result <= 0)
            {
                throw new Exception($"Регистрация не удалась");
            }
            return Created();
        }
        [HttpPost("Login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> Login(LoginModel user)
        {
            var res = await _accessControl.LoginAsync(user.Email, user.Password);

            if (res == null)
            {
                throw new Exception($"Ошибка авторизации");
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays)
            };

            Response.Cookies.Append("refreshToken", res.RefreshToken, cookieOptions);

            var refreshToken = Request.Cookies["refreshToken"];
            return Ok(new RequestAccess
            {
                AccessToken = res.AccessToken,
                RefreshToken = res.RefreshToken,
            });
        }
        [HttpPost("RefreshToken/{accessToken}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RefresToken(string accessToken)
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var tokens = new RequestAccess
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            var newTokens = await _accessControl.GetRefreshTokenAsync(tokens);
            if (newTokens == null)
            {
                throw new Exception($"Обновление токена не удалось");
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays)
            };

            Response.Cookies.Append("refreshToken", newTokens.RefreshToken, cookieOptions);

            return Ok(new
            {   AccessToken = newTokens.AccessToken});
        }
    }
}
