using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user, IEnumerable<string> roles);
    }
}
