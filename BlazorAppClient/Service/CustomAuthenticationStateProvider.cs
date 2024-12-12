using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BlazorAppClient.Service
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly NavigationManager _navigationManager;

        public CustomAuthenticationStateProvider(
            ILocalStorageService localStorageService,
            HttpClient httpClient,
            NavigationManager navigationManager)
        {
            _localStorage = localStorageService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("AccessToken");
            var logoutState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (token != null)
            {
                var claims = GetClaimsPrincipalFromToken(token);
                if(claims == null)
                {
                    await _localStorage.RemoveItemAsync("AccessToken");
                    return logoutState;
                }

                if (string.IsNullOrEmpty(token) || ExpirationTimeIsValid(claims))
                {
                    await _localStorage.RemoveItemAsync("AccessToken");
                    NotifyAuthenticationStateChanged(Task.FromResult(logoutState));
                    return logoutState;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                return new AuthenticationState(claims);

            }
            else
            {
                NotifyAuthenticationStateChanged(Task.FromResult(logoutState));
                return logoutState;
            }

        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            var identity = new ClaimsIdentity(claims, "JWT");

            return new ClaimsPrincipal(identity);
        }

        private bool ExpirationTimeIsValid(ClaimsPrincipal claims)
        {
            // Получаем claim с типом "exp"
            var expirationClaim = claims.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expirationClaim == null)
            {
                return false; 
            }

            if (!long.TryParse(expirationClaim.Value, out long expValue))
            {
                return false; 
            }
            var t = DateTime.Now;

            var expTimeDateUtc = DateTimeOffset.FromUnixTimeSeconds(expValue).UtcDateTime;

            var expTimeDateLocal = expTimeDateUtc.ToLocalTime();

            return expTimeDateLocal < DateTime.Now;
        }


        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsync("AccessToken", token);

            var claimsPrincipal = GetClaimsPrincipalFromToken(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync("AccessToken");
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))));
        }
    }
}
