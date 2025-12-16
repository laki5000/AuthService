using AuthService.Application.Interfaces.Services;
using AuthService.Application.Services;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace AuthService.Application.UnitTests.Services
{
    public class MyAuthServiceTests
    {
        private readonly MyAuthService _authService;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        public MyAuthServiceTests()
        {
            var module = new ApplicationTestModule();

            _authService = module.GetScopedService<MyAuthService>();
            _identityServiceMock = module.IdentityServiceMock;
            _tokenServiceMock = module.TokenServiceMock;
        }

        [Fact]
        public async Task LoginAsync_WhenSuccessful_ShouldReturnOkAndToken()
        {
            var dto = new LoginDto();
            var user = new Mock<IUser>().Object;
            var roles = new List<string>();

            _identityServiceMock.Setup(i => i.ValidateUserCredentialsAsync(dto))
                .ReturnsAsync(user);
            _identityServiceMock.Setup(i => i.GetUserRoles(user))
                .ReturnsAsync(roles);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, roles))
                .Returns(Constants.JWT_TOKEN);

            var result = await _authService.LoginAsync(dto);

            result.Result.Should().Be(Constants.JWT_TOKEN);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task RegisterAsync_WhenSuccessful_ShouldReturnTokenAnd201()
        {
            var dto = new RegisterDto();
            var user = new Mock<IUser>().Object;

            _identityServiceMock.Setup(i => i.CreateUserAsync(dto))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
                .Returns(Constants.JWT_TOKEN);

            var result = await _authService.RegisterAsync(dto);

            result.Result.Should().Be(Constants.JWT_TOKEN);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_WhenSuccessful_ShouldReturnOk()
        {
            var dto = new UpdateUserRoleDto();

            _identityServiceMock.Setup(i => i.UpdateUserRoleAsync(dto))
                .Returns(Task.CompletedTask);

            var result = await _authService.UpdateUserRoleAsync(dto);

            result.StatusCode.Should().Be(StatusCodes.Status200OK);
            result.Result.Should().BeNull();
        }

        [Fact]
        public async Task CreateRoleAsync_WhenSuccessful_ShouldReturnRoleName()
        {

          _identityServiceMock.Setup(i => i.CreateRoleAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(Constants.TEST_ROLE1);

            var result = await _authService.CreateRoleAsync(Constants.TEST_ROLE1);

            result.Result.Should().Be(Constants.TEST_ROLE1);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnRoles()
        {
            var roles = new[] { Constants.TEST_ROLE1, Constants.TEST_ROLE2 };

            _identityServiceMock
                .Setup(i => i.GetAllRolesAsync())
                .ReturnsAsync(roles);

            var result = await _authService.GetAllRolesAsync();

            result.Result.Should().BeEquivalentTo(roles);
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}