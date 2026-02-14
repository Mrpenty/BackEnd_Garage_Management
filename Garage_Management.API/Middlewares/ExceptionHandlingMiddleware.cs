using Garage_Management.Base.Common.Models;
using System.Net;
using System.Text.Json;

namespace Garage_Management.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var payload = ApiResponse<object>.ErrorResponse(ex.Message);
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
            catch (Exception)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var payload = ApiResponse<object>.ErrorResponse("Đã xảy ra lỗi hệ thống.");
                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
        }
    }
}
