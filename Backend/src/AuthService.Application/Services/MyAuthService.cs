using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Services
{
    public class MyAuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public MyAuthService(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<ResultDto<string>> LoginAsync(LoginDto dto)
        {
            var (user, roles) = await _identityService.ValidateUserCredentialsAndGetRolesAsync(dto);
            var token = _tokenService.GenerateToken(user, roles);

            return new ResultDto<string> { Result = token };
        }

        public async Task<ResultDto<string>> RegisterAsync(RegisterDto dto)
        {
            var user = await _identityService.CreateUserAsync(dto);
            var token = _tokenService.GenerateToken(user, Enumerable.Empty<string>());

            return new ResultDto<string>
            {
                Result = token,
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<ResultDto<string>> UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            await _identityService.UpdateUserRoleAsync(dto);
            return new ResultDto<string>();
        }

        public async Task<ResultDto<string>> CreateRoleAsync(string role)
        {
            var createdRole = await _identityService.CreateRoleAsync(role);
            return new ResultDto<string> { Result = createdRole };
        }

        public async Task<ResultDto<IEnumerable<string>>> GetAllRolesAsync()
        {
            var roles = await _identityService.GetAllRolesAsync();
            return new ResultDto<IEnumerable<string>> { Result = roles };
        }
    }
}
