using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<ResultDto<string>> CreateAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ValidationException(ErrorMessages.RoleNameCannotBeEmpty);

            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
                throw new ConflictException(ErrorMessages.RoleAlreadyExists);

            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException(errorMessage);
            }
                
            return new ResultDto<string> { Result = role };
        }

        public ResultDto<IEnumerable<string>> GetAll()
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name!)
                .ToList();

            return new ResultDto<IEnumerable<string>>
            {
                Result = roles ?? Enumerable.Empty<string>()
            };
        }
    }
}
