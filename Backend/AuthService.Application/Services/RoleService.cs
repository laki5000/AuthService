using AuthService.Application.Constants;
using AuthService.Application.Interfaces;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.RoleNameCannotBeEmpty } };

            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.RoleAlreadyExists } };

            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
                return new ResultDto<string> { Success = false, Errors = result.Errors.Select(e => e.Description) };

            return new ResultDto<string> { Success = true, Result = role };
        }

        public async Task<ResultDto<IEnumerable<string>>> GetAllAsync()
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            return new ResultDto<IEnumerable<string>>()
            {
                Success = true,
                Result = roles ?? Enumerable.Empty<string>()
            };
        }
    }
}
