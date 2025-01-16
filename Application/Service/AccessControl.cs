using Infrasrtucture.Helpers;
using Application.Models;
using Entities.Interfaces;
using Entities.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Service
{
    public class AccessControl<TUser> where TUser : User
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IRoleManager<TUser> _roleManager;
        private readonly IUserManager<TUser> _userManager;
        private readonly IRefreshTokenManager _tokenManager;
        public AccessControl(JwtSettings jwtSettings, IRoleManager<TUser> roleManager, IUserManager<TUser> userManager, IRefreshTokenManager tokenManager)
        {
            _jwtSettings = jwtSettings;
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenManager = tokenManager;
        }


        public async Task<int> RegistrationAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException($"Пользователь не может быть пустым");
            }
            var addUser = await _userManager.CreateUserAsync(user);
            return addUser==null ? 0 : 1;
        }

        public async Task<RequestAccess> LoginAsync(string email, string password)
        {
            var existsUser = await _userManager.GetUserByEmailAsync(email);
            if (existsUser == null)
            {
                throw new ArgumentException($"Неверный логин или пароль");
            }

            if (!PasswordHasher.VerifyPassword(existsUser.PasswordHash, password))
            {
                throw new ArgumentException($"Неверный логин или пароль");
            }

            if (existsUser.IsLocked == true)
            {
                bool isBlockNow = existsUser.BlockedUntil > DateTime.UtcNow;
                if (isBlockNow)
                {
                    throw new ArgumentException($"Пользователь заблокирован {existsUser.BlockedUntil}");
                }
                existsUser.IsLocked = false;

            }
            var existSessions = await _userManager.CheckUserSessions(existsUser.UserId);
            if(existSessions)
            {
                throw new Exception($"Слишком много открытых сессий. Попробуйте войти еще раз");
            }
            var requestTokens = await GenerateTokens(existsUser);
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
                throw new Exception($"Invalid access");
            }
            return requestTokens;
        }

        public async Task<RequestAccess?> GetRefreshTokenAsync(RequestAccess request)
        {
            var principal = await GetClaimsPrincipalAsync(request.AccessToken);
            if (principal == null)
            {
                return null;
            }
            var userEmail = principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new Exception($"Пользователя нет в токене");
            }
            var user = await _userManager.GetUserByEmailAsync(userEmail);
            if (user == null || user.IsLocked)
            {
                throw new Exception($"Пользователь не найден или заблокирован");
            }
            var refreshToken = await _tokenManager.GetRefreshTokenAsync(request.RefreshToken, user.UserId);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new Exception($"Ошибка или срок действия токена истек");
            }
            var requestAccess = await GenerateTokens(user);

            refreshToken.Token = requestAccess.RefreshToken;
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityDays);
            refreshToken.CreatedDate = DateTime.UtcNow;
            refreshToken.IsRevoked = false;

            await _tokenManager.AddRefreshTokenAsync(refreshToken);
            return requestAccess;
        }
        private async Task<RequestAccess> GenerateTokens(TUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.TokenValidityMinutes);
            var claims = await GetClaims(user);
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
                );

            return new RequestAccess
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = GenerateRefreshToken()
            };
        }

        private string GenerateRefreshToken()
        {
            var rndBytes = new byte[64];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(rndBytes);
            }
            return Convert.ToBase64String(rndBytes);
        }

        public virtual async Task<ClaimsPrincipal> GetClaimsPrincipalAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (jwtToken == null)
                {
                    return null;
                }
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
                };
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public virtual async Task<List<Claim>> GetClaims(TUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };
            
            var roles = await _roleManager.GetUserRolesByIdAsync(user.UserId);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }
    }
}
