using AuthService.Application.Interfaces;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public UserService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ResultDto<string>> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
                return new ResultDto<string> { Success = false, Errors = new[] { "Invalid credentials" } };

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return new ResultDto<string> { Success = false, Errors = new[] { "Invalid credentials" } };

            var token = await _tokenService.GenerateTokenAsync(user);
            return new ResultDto<string> { Success = true, Result = token };
        }

        public async Task<ResultDto<string>> RegisterAsync(RegisterDto dto)
        {
            var existing = await _userManager.FindByNameAsync(dto.Username);
            if (existing != null)
                return new ResultDto<string> { Success = false, Errors = new[] { "Username already exists" } };

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return new ResultDto<string> { Success = false, Errors = createResult.Errors.Select(e => e.Description) };

            var token = await _tokenService.GenerateTokenAsync(user);
            return new ResultDto<string> { Success = true, Result = token };
        }
    }
}
