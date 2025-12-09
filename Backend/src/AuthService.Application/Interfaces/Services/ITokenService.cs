using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
