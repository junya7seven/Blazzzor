using Shared;
using System.Text.Json;

namespace BlazorAppClient.Service.ErrorHelper
{
    public static class ErrorParser
    {
        public static string ErrorMessage(string contextMessage)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(contextMessage);
            Console.WriteLine(errorResponse.ErrorMessage);

            return errorResponse?.ErrorMessage ?? $"Неизвестная ошибка статус код: {errorResponse.StatusCode}";
        }
    }
}