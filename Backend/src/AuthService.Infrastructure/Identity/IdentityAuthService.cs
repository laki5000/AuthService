using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity
{
    public class IdentityAuthService : IAuthService
    {
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly ITokenService _tokenService;

        public IdentityAuthService(
        UserManager<MyIdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<MyIdentityUser> signInManager,
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

            var user = new MyIdentityUser
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

        public async Task<ResultDto<string>> UpdateUserRoleAsync(string username, string role, bool add)
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

        public async Task<ResultDto<string>> CreateRoleAsync(string role)
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

        public ResultDto<IEnumerable<string>> GetAllRolesAsync()
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
