using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<(IUser user, IEnumerable<string> roles)> ValidateUserCredentialsAndGetRolesAsync(LoginDto dto);
        Task<IUser> CreateUserAsync(RegisterDto dto);
        Task UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<string> CreateRoleAsync(string role);
        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
