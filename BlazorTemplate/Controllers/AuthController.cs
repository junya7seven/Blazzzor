using Application;
using Application.Models;
using Application.Models.AuthModels;
using Application.Service;
using AutoMapper;
using BlazorTemplateAPI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BlazorTemplateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccessControl _accessControl;
        private readonly JwtSettings _jwtSettings;
        public AuthController(AccessControl accessControl, JwtSettings jwtSettings)
        {
            _accessControl = accessControl;
            _jwtSettings = jwtSettings;
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
            var result = await _accessControl.RegistrationAsync(model);
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
            { AccessToken = newTokens.AccessToken });
        }
    }
}
