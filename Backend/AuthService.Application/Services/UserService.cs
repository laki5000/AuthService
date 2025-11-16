using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
                throw new AuthenticationException(ErrorMessages.InvalidCredentials);

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new AuthenticationException(ErrorMessages.InvalidCredentials);

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(user, roles);
            return new ResultDto<string> { Result = token };
        }

        public async Task<ResultDto<string>> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Username);
            if (existing != null)
                throw new ConflictException(ErrorMessages.UsernameAlreadyExists);

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errorMessage = string.Join(", ", createResult.Errors.Select(e => e.Description));
                throw new OperationFailedException(errorMessage);
            }

            var token = _tokenService.GenerateToken(user, Enumerable.Empty<string>());
            return new ResultDto<string> { Result = token, StatusCode = StatusCodes.Status201Created };
        }

        public async Task<ResultDto<string>> UpdateUserRole(string username, string role, bool add)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            if (!await _roleManager.RoleExistsAsync(role))
                throw new NotFoundException(ErrorMessages.RoleDoesNotExist(role));

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
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException(errorMessage);
            }

            return new ResultDto<string>();
        }
    }
}
