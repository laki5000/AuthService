using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ITokenHeaderService _tokenHeaderService;

        public JwtCookieMiddleware(RequestDelegate next, ITokenHeaderService tokenHeaderService)
        {
            _next = next;
            _tokenHeaderService = tokenHeaderService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _tokenHeaderService.AddJwtFromCookieToHeader(context);

            await _next(context);
        }
    }
}
