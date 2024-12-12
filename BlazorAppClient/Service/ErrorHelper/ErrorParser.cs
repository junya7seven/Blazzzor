using System.Text.Json;

namespace BlazorAppClient.Service.ErrorHelper
{
    public static class ErrorParser
    {
        public static string ErrorMessage(string contextMessage)
        {
            var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(contextMessage);
            Console.WriteLine(errorResponse.message);

            return errorResponse?.message ?? $"Неизвестная ошибка статус код: {errorResponse.statusCode}";
        }
    }
    public class ErrorResponse
    {
        public int statusCode { get; set; }
        public string message { get; set; }
    }
}