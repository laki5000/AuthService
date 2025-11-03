using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITokenHeaderService
    {
        void AddJwtFromCookieToHeader(HttpContext context);
    }
}
