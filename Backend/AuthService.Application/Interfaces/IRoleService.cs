using AuthService.Domain.DTOs;

namespace AuthService.Application.Interfaces
{
    public interface IRoleService
    {
        Task<ResultDto<string>> CreateAsync(string role);
        Task<ResultDto<IEnumerable<string>>> GetAllAsync();
    }
}
