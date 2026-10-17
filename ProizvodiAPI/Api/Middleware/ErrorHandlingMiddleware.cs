using System.Net;
using System.Text.Json;

namespace ProizvodiAPI.Api.MIddleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Prosleđujemo request sledećem middlewareu u pipeline-u
                await _next(context);
            }
            catch (Exception ex)
            {
                // Hvatamo SVE neočekivane greške
                _logger.LogError(ex, "Neočekivana greška: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var response = ex switch
            {
                ArgumentException => new ErrorResponse(
                    (int)HttpStatusCode.BadRequest,
                    "Neispravan zahtev.",
                    ex.Message),

                KeyNotFoundException => new ErrorResponse(
                    (int)HttpStatusCode.NotFound,
                    "Resurs nije pronađen.",
                    ex.Message),

                InvalidOperationException => new ErrorResponse(
                    (int)HttpStatusCode.Conflict,
                    "Operacija nije moguća u trenutnom stanju.",
                    ex.Message),

                UnauthorizedAccessException => new ErrorResponse(
                    (int)HttpStatusCode.Unauthorized,
                    "Nemate pristup ovom resursu.",
                    ex.Message),

                _ => new ErrorResponse(
                    (int)HttpStatusCode.InternalServerError,
                    "Došlo je do greške na serveru.",
                    "Molimo pokušajte ponovo kasnije.")
            };

            context.Response.StatusCode = response.StatusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }

        public record ErrorResponse(int StatusCode, string Poruka, string Detalji);
    }
}
