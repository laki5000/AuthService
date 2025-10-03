﻿using AuthService.Application.Constants;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.DTOs;
using AuthService.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace AuthService.Application.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        private readonly UserService _userService;

        public UserServiceTests() {
            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                new IUserValidator<User>[] { new Mock<IUserValidator<User>>().Object },
                new IPasswordValidator<User>[] { new Mock<IPasswordValidator<User>>().Object },
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );

            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new List<IRoleValidator<IdentityRole>> { new Mock<IRoleValidator<IdentityRole>>().Object },
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            );

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object
            );

            _tokenServiceMock = new Mock<ITokenService>();
            _userService = new UserService(_userManagerMock.Object, _roleManagerMock.Object, _signInManagerMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidUsername_ShouldReturnUnauthorized()
        {
            var dto = new LoginDto { Username = Constants.NONEXISTENT_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync((User?)null);

            var result = await _userService.LoginAsync(dto);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.InvalidCredentials);
            result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldReturnUnauthorized()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.INVALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
                              .ReturnsAsync(SignInResult.Failed);

            var result = await _userService.LoginAsync(dto);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.InvalidCredentials);
            result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            var roles = new List<string> { Constants.TEST_ROLE1, Constants.TEST_ROLE2 };
            var token = Constants.JWT_TOKEN;
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.VALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false))
                              .ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(u => u.GetRolesAsync(user))
                            .ReturnsAsync(roles);
            _tokenServiceMock.Setup(t => t.GenerateToken(user, roles))
                             .Returns(token);

            var result = await _userService.LoginAsync(dto);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(token);
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameAlreadyExists_ShouldReturnConflict()
        {
            var dto = new RegisterDto { Username = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync(new User());

            var result = await _userService.RegisterAsync(dto);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.UsernameAlreadyExists);
            result.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task RegisterAsync_WhenCreateFails_ShouldReturnBadRequest()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync((User?)null);

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            var result = await _userService.RegisterAsync(dto);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(Constants.ERROR_MESSAGE);
            result.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterAsync_WhenSuccessful_ShouldReturnCreatedAndToken()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync((User?)null);

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
                             .Returns(Constants.JWT_TOKEN);

            var result = await _userService.RegisterAsync(dto);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(Constants.JWT_TOKEN);
            result.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public async Task UpdateUserRole_WhenUserNotFound_ShouldReturnNotFound()
        {
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.NONEXISTENT_USERNAME))
                            .ReturnsAsync((User?)null);

            var result = await _userService.UpdateUserRole(Constants.NONEXISTENT_USERNAME, Constants.TEST_ROLE1, true);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.UserNotFound);
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateUserRole_WhenRoleDoesNotExist_ShouldReturnNotFound()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.NONEXISTENT_ROLE)).ReturnsAsync(false);

            var result = await _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.NONEXISTENT_ROLE, true);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.RoleDoesNotExist(Constants.NONEXISTENT_ROLE));
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateUserRole_WhenAddOrRemoveFails_ShouldReturnBadRequest()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);

            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            var result = await _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.TEST_ROLE1, true);

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(Constants.ERROR_MESSAGE);
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task UpdateUserRole_WhenAddOrRemoveSucceeds_ShouldReturnOk()
        {
            var user = new User { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);

            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1))
                            .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.UpdateUserRole(Constants.EXISTING_USERNAME, Constants.TEST_ROLE1, true);

            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }
    }
}
