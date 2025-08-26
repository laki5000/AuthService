using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateTokenAsync(User user, IEnumerable<string> roles);
    }
}
