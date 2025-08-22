using AuthService.Domain.DTOs;

namespace AuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}
