using AuthService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace AuthService.Infrastructure.Security
{
    public class TokenHeaderService : ITokenHeaderService
    {
        public static readonly string JwtCookieName = "jwt";
        public static readonly string AuthorizationHeaderName = "Authorization";
        public static readonly string BearerPrefix = "Bearer ";

        public void AddJwtFromCookieToHeader(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue(JwtCookieName, out var token)
                && !context.Request.Headers.ContainsKey(AuthorizationHeaderName))
            {
                context.Request.Headers.Append(AuthorizationHeaderName, $"{BearerPrefix}{token}");
            }
        }
    }
}
