using System.Net;
using System.Text.Json;

namespace ProductApp.WebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex.Message}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string errorMessage = "Internal Server Error. Please try again later.";

            switch (exception)
            {
                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorMessage = "Resource not found.";
                    break;
                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorMessage = "Invalid input provided.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = errorMessage
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
