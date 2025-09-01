using AuthService.Application.Constants;
using AuthService.Application.Interfaces;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public UserService(
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<User> signInManager,
        ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ResultDto<string>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.InvalidCredentials }, StatusCode = (int)HttpStatusCode.Unauthorized };

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.InvalidCredentials }, StatusCode = (int)HttpStatusCode.Unauthorized };

            var roles = await _userManager.GetRolesAsync(user);

            var token = await _tokenService.GenerateTokenAsync(user, roles);
            return new ResultDto<string> { Success = true, Result = token };
        }

        public async Task<ResultDto<string>> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Username);
            if (existing != null)
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.UsernameAlreadyExists }, StatusCode = (int)HttpStatusCode.Conflict };

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return new ResultDto<string> { Success = false, Errors = createResult.Errors.Select(e => e.Description), StatusCode = (int)HttpStatusCode.BadRequest };

            var token = await _tokenService.GenerateTokenAsync(user, Enumerable.Empty<string>());
            return new ResultDto<string> { Success = true, Result = token, StatusCode = (int)HttpStatusCode.Created };
        }

        public async Task<ResultDto<string>> UpdateUserRole(string username, string role, bool add)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.UserNotFound }, StatusCode = 404 };
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new ResultDto<string> { Success = false, Errors = new[] { ErrorMessages.RoleDoesNotExist(role) }, StatusCode = 404 };
            }

            IdentityResult result;

            if (add)
            {
                result = await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                result = await _userManager.RemoveFromRoleAsync(user, role);
            }

            if (!result.Succeeded)
            {
                return new ResultDto<string> { Success = false, Errors = result.Errors.Select(e => e.Description).ToArray(), StatusCode = 400 };
            }

            return new ResultDto<string> { Success = true, StatusCode = 200 };
        }
    }
}
