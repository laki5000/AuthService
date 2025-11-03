using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public UserController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);

            if (result.Success)
            {
                var jwtSection = _config.GetSection("Jwt");

                Response.Cookies.Append("jwt", result.Result!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSection["ExpiresMinutes"] ?? "60"))
                });
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(ResultDto<string>), StatusCodes.Status200OK)]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("jwt"))
            {
                Response.Cookies.Delete("jwt");
            }

            var result = new ResultDto<string>()
            {
                Success = true
            };
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
