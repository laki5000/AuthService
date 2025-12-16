using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
namespace AuthService.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;

        public IdentityService(
        UserManager<MyIdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<MyIdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<(IUser user, IEnumerable<string> roles)> ValidateUserCredentialsAndGetRolesAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username)
                ?? throw new AuthenticationException(ErrorMessages.InvalidCredentials);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new AuthenticationException(ErrorMessages.InvalidCredentials);

            var roles = await _userManager.GetRolesAsync(user);

            return (user, roles);
        }

        public async Task<IUser> CreateUserAsync(RegisterDto dto)
        {
            if (await _userManager.FindByNameAsync(dto.Username) is not null)
                throw new ConflictException(ErrorMessages.UsernameAlreadyExists);

            var user = new MyIdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            EnsureSuccess(result);

            return user;
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username)
                ?? throw new NotFoundException(ErrorMessages.UserNotFound);


            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
                throw new NotFoundException(ErrorMessages.RoleDoesNotExist(dto.RoleName));

            IdentityResult result = dto.Add
                ? await _userManager.AddToRoleAsync(user, dto.RoleName)
                : await _userManager.RemoveFromRoleAsync(user, dto.RoleName);

            EnsureSuccess(result);
        }

        public async Task<string> CreateRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ValidationException(ErrorMessages.RoleNameCannotBeEmpty);

            if (await _roleManager.RoleExistsAsync(role))
                throw new ConflictException(ErrorMessages.RoleAlreadyExists);

            var result = await _roleManager.CreateAsync(new IdentityRole(role));
            EnsureSuccess(result);

            return role;
        }

        public Task<IEnumerable<string>> GetAllRolesAsync()
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name!)
                .ToList();

            return Task.FromResult<IEnumerable<string>>(roles);
        }

        private static void EnsureSuccess(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException(errorMessage);
            }
        }
    }
}
