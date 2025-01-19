using Application.Helpers;
using Application.Interfaces;
using Application.Models;
using Application.Models.AuthModels;
using AutoMapper;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Service
{
    public class AccessControl
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenManager _tokenManager;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccessControl(JwtSettings jwtSettings, IUserService userService,
            IRefreshTokenManager tokenManager, ITokenService tokenService)
        {
            _userService = userService;
            _jwtSettings = jwtSettings;
            _tokenManager = tokenManager;
            _tokenService = tokenService;
        }

        public async Task<int> RegistrationAsync(RegistrationModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException($"Пользователь не может быть пустым");
            }
            var addUser = await _userService.CreateUserAsync(user);
            return addUser == null ? 0 : 1;
        }

        public async Task<RequestAccess> LoginAsync(string email, string password)
        {
            var existsUser = await _userService.GetUserByEmailAsync(email);
            if (existsUser == null)
            {
                throw new ArgumentNullException($"Такого пользователя не существует");
            }

            if (!PasswordHasher.VerifyPassword(existsUser.PasswordHash, password))
            {
                throw new ArgumentException($"Неверный логин или пароль");
            }

            if (existsUser.IsLocked && existsUser.BlockedUntil > DateTime.UtcNow)
            {
                throw new ArgumentException($"Пользователь заблокирован до {existsUser.BlockedUntil}");
            }

            //await _userService.UnlockUserIfBlockedAsync(existsUser);

            var requestTokens = await _tokenService.GenerateTokens(existsUser);
            var tokenEntry = new RefreshToken
            {
                Token = requestTokens.RefreshToken,
                UserId = existsUser.UserId,
                ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays),
                CreatedDate = DateTime.UtcNow,
                IsRevoked = false,
            };

            if (!await _tokenManager.AddRefreshTokenAsync(tokenEntry))
            {
                throw new Exception("Не удалось сохранить токен.");
            }

            return requestTokens;
        }

        public async Task<RequestAccess?> GetRefreshTokenAsync(RequestAccess request)
        {
            var principal = await _tokenService.GetClaimsPrincipalAsync(request.AccessToken);
            if (principal == null)
            {
                return null;
            }
            var userEmail = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new Exception($"Пользователя нет в токене");
            }
            var user = await _userService.GetUserByEmailAsync(userEmail);
            if (user == null || user.IsLocked)
            {
                throw new Exception($"Пользователь не найден или заблокирован");
            }
            var refreshToken = await _tokenManager.GetRefreshTokenAsync(request.RefreshToken, user.UserId);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new Exception($"Ошибка или срок действия токена истек");
            }

            var requestAccess = await _tokenService.GenerateTokens(user);

            refreshToken.Token = requestAccess.RefreshToken;
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays);
            refreshToken.CreatedDate = DateTime.UtcNow;
            refreshToken.IsRevoked = false;

            await _tokenManager.AddRefreshTokenAsync(refreshToken);
            return requestAccess;
        }
    }

}
