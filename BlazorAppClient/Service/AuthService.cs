using BlazorAppClient.Service.ErrorHelper;
using Blazored.LocalStorage;
using Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorAppClient.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly CustomAuthenticationStateProvider _customAuthenticationStateProvider;
        private ErrorResponse _errorResponse;

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage, 
            CustomAuthenticationStateProvider customAuthenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _customAuthenticationStateProvider = customAuthenticationStateProvider;
        }

        public async Task<bool> RegistrationAsync(RegistrationUser user)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/Registration", user);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            var error = await response.Content.ReadAsStringAsync();
            _errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception($"Ошибка регистрации: {_errorResponse.ErrorMessage}");
        }

        public async Task<bool> LoginAsync(LoginUser user)
        {
            string error;
            var response = await _httpClient.PostAsJsonAsync("Auth/Login", user);
            if (!response.IsSuccessStatusCode)
            {
                _errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
                throw new Exception($"{_errorResponse.ErrorMessage}");
            }
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                await SetTokenAsync(result);
                var data = await _localStorage.GetItemAsStringAsync("AccessToken");
                await _customAuthenticationStateProvider.GetAuthenticationStateAsync();

                if(data == null)
                {
                    error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка входа: не удалось сохранить токен");
                }
                return true;
            }
            error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка входа: не удалось получить токен");
        }

        private async Task SetTokenAsync(TokenResponse response)
        {
            if(response != null)
            {
                await _localStorage.SetItemAsStringAsync("AccessToken", response.AccessToken);
            }
            else
            {
                throw new Exception("Не удалось записать сохранить токен");
            }
        }

        


    }
}
