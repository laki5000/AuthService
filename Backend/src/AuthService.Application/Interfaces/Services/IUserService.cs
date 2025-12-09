using AuthService.Domain.DTOs;

namespace AuthService.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ResultDto<string>> RegisterAsync(RegisterDto dto);
        Task<ResultDto<string>> LoginAsync(LoginDto dto);
        Task<ResultDto<string>> UpdateUserRole(string username, string role, bool add);
    }
}
