using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace AuthService.Infrastructure.Middleware
{
    public class CustomResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            if ((context.Response.StatusCode == (int)HttpStatusCode.Unauthorized || 
                context.Response.StatusCode == (int)HttpStatusCode.Forbidden) &&
                memoryStream.Length == 0)
            {
                context.Response.ContentType = "application/json";

                var result = new ResultDto<string>
                {
                    Success = false,
                    Errors = new[]
                    {
                    context.Response.StatusCode == 401
                        ? "Login failed or token is invalid."
                        : "You do not have the required permissions."
                },
                    StatusCode = context.Response.StatusCode
                };

                var json = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(json);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                await context.Response.Body.CopyToAsync(originalBodyStream);
            }
            else
            {
                memoryStream.Seek(0, SeekOrigin.Begin);

                var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                var resultDto = JsonSerializer.Deserialize<ResultDto<object>>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (resultDto != null)
                {
                    context.Response.StatusCode = resultDto.StatusCode;
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
            }
        }
    }
}
