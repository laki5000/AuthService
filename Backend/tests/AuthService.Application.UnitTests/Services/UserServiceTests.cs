using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Services;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthService.Application.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        public UserServiceTests()
        {
            var module = new ApplicationTestModule();

            _userService = module.UserService;
            _userManagerMock = module.UserManagerMock;
            _roleManagerMock = module.RoleManagerMock;
            _signInManagerMock = module.SignInManagerMock;
            _tokenServiceMock = module.TokenServiceMock;
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ShouldThrowAuthenticationException()
        {
            var dto = new LoginDto { Username = Constants.NONEXISTENT_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);

            await FluentActions.Invoking(() => _userService.LoginAsync(dto))
                .Should().ThrowAsync<AuthenticationException>()
                .WithMessage(ErrorMessages.InvalidCredentials);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowAuthenticationException()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.INVALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(SignInResult.Failed);

            await FluentActions.Invoking(() => _userService.LoginAsync(dto))
                .Should().ThrowAsync<AuthenticationException>()
                .WithMessage(ErrorMessages.InvalidCredentials);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            var roles = new List<string> { Constants.TEST_ROLE1, Constants.TEST_ROLE2 };
            var token = Constants.JWT_TOKEN;
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.VALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, roles)).Returns(token);

            var result = await _userService.LoginAsync(dto);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(token);
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameAlreadyExists_ShouldThrowConflictException()
        {
            var dto = new RegisterDto { Username = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(new User());

            await FluentActions.Invoking(() => _userService.RegisterAsync(dto))
                .Should().ThrowAsync<ConflictException>()
                .WithMessage(ErrorMessages.UsernameAlreadyExists);
        }

        [Fact]
        public async Task RegisterAsync_WhenCreateFails_ShouldThrowOperationFailedException()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            await FluentActions.Invoking(() => _userService.RegisterAsync(dto))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task RegisterAsync_WhenSuccessful_ShouldReturnCreatedAndToken()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password)).ReturnsAsync(IdentityResult.Success);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>())).Returns(Constants.JWT_TOKEN);

            var result = await _userService.RegisterAsync(dto);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(Constants.JWT_TOKEN);
            result.StatusCode.Should().Be(StatusCodes.Status201Created);
        }

        [Fact]
        public async Task UpdateUserRole_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.NONEXISTENT_USERNAME)).ReturnsAsync((User?)null);

            await FluentActions.Invoking(() => _userService.UpdateUserRole(Constants.NONEXISTENT_USERNAME, Constants.TEST_ROLE1, true))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage(ErrorMessages.UserNotFound);
        }

        [Fact]
        public async Task UpdateUserRole_WhenRoleDoesNotExist_ShouldThrowNotFoundException()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.NONEXISTENT_ROLE)).ReturnsAsync(false);

            await FluentActions.Invoking(() => _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.NONEXISTENT_ROLE, true))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage(ErrorMessages.RoleDoesNotExist(Constants.NONEXISTENT_ROLE));
        }

        [Fact]
        public async Task UpdateUserRole_WhenAddOrRemoveFails_ShouldThrowOperationFailedException()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);

            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            await FluentActions.Invoking(() => _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.TEST_ROLE1, true))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task UpdateUserRole_WhenAddOrRemoveSucceeds_ShouldReturnOk()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1)).ReturnsAsync(IdentityResult.Success);

            var result = await _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.TEST_ROLE1, true);

            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}
