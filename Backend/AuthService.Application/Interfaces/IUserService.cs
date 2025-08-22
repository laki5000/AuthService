using AuthService.Domain.DTOs;

namespace AuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<string>> RegisterAsync(RegisterDto dto);
        Task<ResultDto<string>> LoginAsync(LoginDto dto);
    }
}
