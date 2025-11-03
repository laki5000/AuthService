using AuthService.Application.Interfaces.Services;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] string roleName)
        {
            var result = await _roleService.CreateAsync(roleName);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ResultDto<IEnumerable<string>>), StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var result = _roleService.GetAll();
            return Ok(result);
        }
    }
}
