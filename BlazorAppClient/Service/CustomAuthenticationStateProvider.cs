using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using LibaryModalDialogPages.Interface;
using System.Net.Http.Json;

namespace BlazorAppClient.Service
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider,ICustomAuthenticationStateProvider
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

            if (!string.IsNullOrEmpty(token))
            {

                if (TokenExpired(token))
                {
                    try
                    {
                        var newToken = await RefreshToken(token);
                        if (!string.IsNullOrEmpty(newToken))
                        {
                            token = newToken;
                            await _localStorage.SetItemAsync("AccessToken", token);
                        }
                        else
                        {
                            return logoutState;
                        }
                    }
                    catch
                    {
                        return logoutState;
                    }
                }

                var identity = GetClaimsPrincipalFromToken(token);
                if(identity == null)
                {
                    await _localStorage.RemoveItemAsync("AccessToken");
                    return logoutState;
                }

                SetAuthorizationHeader(token);

                var newAuth = new AuthenticationState(identity);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(identity)));
                return newAuth;

            }
            else
            {
                NotifyAuthenticationStateChanged(Task.FromResult(logoutState));
                return logoutState;
            }
        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            try
            {
                var jwtToken = _tokenHandler.ReadJwtToken(token);
                var claims = jwtToken.Claims;
                var identity = new ClaimsIdentity(claims, "JWT");
                return new ClaimsPrincipal(identity);
            }
            catch (Exception)
            {
                return null;
            }

        }

        private void SetAuthorizationHeader(string token)
        {
            if (_httpClient.DefaultRequestHeaders.Authorization?.Parameter != token)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        private bool TokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            return jwtToken?.ValidTo < DateTime.UtcNow;
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

        private async Task<string> RefreshToken(string token)
        {
            var response = await _httpClient.PostAsync($"Auth/RefreshToken/{token}", null);
            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokens != null)
                {
                    return tokens.AccessToken;
                }
            }
            return null;
        }
    }
}
