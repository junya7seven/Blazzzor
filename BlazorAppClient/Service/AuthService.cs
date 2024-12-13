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

        public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<bool> RegistrationAsync(RegistrationUser user)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/Registration", user);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            var error = await response.Content.ReadAsStringAsync();
            error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка входа: {ErrorParser.ErrorMessage(error)}");
        }

        public async Task<bool> LoginAsync(LoginUser user)
        {
            string error;
            var response = await _httpClient.PostAsJsonAsync("Auth/Login", user);
            if (!response.IsSuccessStatusCode)
            {
                error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Ошибка входа: {ErrorParser.ErrorMessage(error)}");
            }
            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                await SetTokenAsync(result);
                var data = await _localStorage.GetItemAsStringAsync("AccessToken");


                if(data == null)
                {
                    error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Ошибка входа: {ErrorParser.ErrorMessage(error)}");
                }
                return true;
            }
            error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Ошибка входа: {ErrorParser.ErrorMessage(error)}");
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

        public async Task<string> RefreshToken(string token)
        {
            var response = await _httpClient.PostAsync($"Auth/RefreshToken/{token}", null);
            if (response.IsSuccessStatusCode)
            {
                var tokens = await response.Content.ReadFromJsonAsync<TokenResponse>();
                if (tokens != null)
                {
                    await SetTokenAsync(tokens);
                    return tokens.AccessToken;
                }
            }
            return null;
        }


    }
}
