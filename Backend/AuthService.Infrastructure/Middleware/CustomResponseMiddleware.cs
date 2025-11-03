using AuthService.Application.Interfaces.Common;
using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Middleware
{
    public class CustomResponseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IResponseHandler _responseHandler;

        private const string JsonContentType = "application/json";

        public CustomResponseMiddleware(RequestDelegate next, IResponseHandler responseHandler)
        {
            _next = next;
            _responseHandler = responseHandler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await _next(context);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(memoryStream).ReadToEndAsync();

            var (handled, jsonResponse, newStatusCode) =
                _responseHandler.HandleResponse(context.Response.StatusCode, body);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType = JsonContentType;

            if (newStatusCode.HasValue)
                context.Response.StatusCode = newStatusCode.Value;

            if (handled && jsonResponse != null)
                await context.Response.WriteAsync(jsonResponse);
            else
                await context.Response.WriteAsync(body);
        }
    }
}
