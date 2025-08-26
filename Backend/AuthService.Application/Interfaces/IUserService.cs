using AuthService.Domain.DTOs;
using System.Data;

namespace AuthService.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<string>> RegisterAsync(RegisterDto dto);
        Task<ResultDto<string>> LoginAsync(LoginDto dto);
        Task<ResultDto<string>> UpdateUserRole(string username, string role, bool add);
    }
}
