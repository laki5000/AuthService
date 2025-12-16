using AuthService.Domain.DTOs;

namespace AuthService.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ResultDto<string>> RegisterAsync(RegisterDto dto);
        Task<ResultDto<string>> LoginAsync(LoginDto dto);
        Task<ResultDto<string>> UpdateUserRoleAsync(string username, string role, bool add);
        Task<ResultDto<string>> CreateRoleAsync(string role);
        ResultDto<IEnumerable<string>> GetAllRolesAsync();
    }
}
