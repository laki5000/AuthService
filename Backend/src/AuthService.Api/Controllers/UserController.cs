using AuthService.Application.Helpers;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        private readonly JwtOptions _jwtOptions;

        public UserController(IUserService userService, IOptions<JwtOptions> jwtOptions)
        {
            _userService = userService;

            _jwtOptions = jwtOptions.Value;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);

            CookieHelper.SetJwtCookie(Response, result.Result!, _jwtOptions.ExpiresMinutes);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);

            CookieHelper.SetJwtCookie(Response, result.Result!, _jwtOptions.ExpiresMinutes);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ResultDto<string>), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            CookieHelper.DeleteJwtCookie(Request, Response);

            var result = new ResultDto<string>();
            return Ok(result);
        }

        [HttpGet("checkAuth")]
        [Authorize]
        [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status200OK)]
        public IActionResult CheckAuth()
        {
            var result = new ResultDto<bool> { Result = true };
            return Ok(result);
        }

        [HttpPost("updateUserRole/{username}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserRole(string username, [FromQuery] string role, [FromQuery] bool add = true)
        {
            var result = await _userService.UpdateUserRole(username, role, add);
            return Ok(result);
        }

        [HttpGet("amIAdmin")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResultDto<string>), StatusCodes.Status200OK)]
        public IActionResult AmIAdmin()
        {
            var result = new ResultDto<string>()
            {
                Success = true
            };
            return Ok(result);
        }
    }
}
