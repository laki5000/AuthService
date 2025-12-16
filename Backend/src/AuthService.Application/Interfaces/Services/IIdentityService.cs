using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<IUser> ValidateUserCredentialsAsync(LoginDto dto);
        Task<IEnumerable<string>> GetUserRoles(IUser user);
        Task<IUser> CreateUserAsync(RegisterDto dto);
        Task UpdateUserRoleAsync(UpdateUserRoleDto dto);
        Task<string> CreateRoleAsync(string role);
        Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
