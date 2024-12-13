using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using LibaryModalDialogPages.Interface;

namespace BlazorAppClient.Service
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider,ICustomAuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly NavigationManager _navigationManager;
        private readonly AuthService _authService;

        public CustomAuthenticationStateProvider(
            ILocalStorageService localStorageService,
            HttpClient httpClient,
            NavigationManager navigationManager,
            AuthService authService)
        {
            _localStorage = localStorageService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _authService = authService;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsStringAsync("AccessToken");
            var logoutState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            if (token != null)
            {

                if (TokenExpired(token))
                {
                    var tokens = await _authService.RefreshToken(token);
                    if(string.IsNullOrEmpty(tokens))
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

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                return new AuthenticationState(identity);

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
    }
}
