using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Domain.DTOs;
using AuthService.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthService.Infrastructure.UnitTests.Identity
{
    public  class IdentityServiceTests
    {
        private readonly IdentityService _identityService;
        private readonly Mock<UserManager<MyIdentityUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<SignInManager<MyIdentityUser>> _signInManagerMock;

        public IdentityServiceTests()
        {
            var module = new InfrastructureTestModule();

            _identityService = module.GetScopedService<IdentityService>();
            _userManagerMock = module.UserManagerMock;
            _roleManagerMock = module.RoleManagerMock;
            _signInManagerMock = module.SignInManagerMock;
        }

        [Fact]
        public async Task ValidateUserCredentialsAndGetRolesAsync_WithInvalidUsername_ShouldThrowAuthenticationException()
        {
            var dto = new LoginDto { Username = Constants.NONEXISTENT_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((MyIdentityUser?)null);

            await FluentActions.Invoking(() => _identityService.ValidateUserCredentialsAndGetRolesAsync(dto))
                .Should().ThrowAsync<AuthenticationException>()
                .WithMessage(ErrorMessages.InvalidCredentials);
        }

        [Fact]
        public async Task ValidateUserCredentialsAndGetRolesAsync_WithWrongPassword_ShouldThrowAuthenticationException()
        {
            var user = new MyIdentityUser { UserName = Constants.EXISTING_USERNAME };
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.INVALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(SignInResult.Failed);

            await FluentActions.Invoking(() => _identityService.ValidateUserCredentialsAndGetRolesAsync(dto))
                .Should().ThrowAsync<AuthenticationException>()
                .WithMessage(ErrorMessages.InvalidCredentials);
        }

        [Fact]
        public async Task ValidateUserCredentialsAndGetRolesAsync_WithValidCredentials_ShouldReturnUserAndRoles()
        {
            var user = new MyIdentityUser { UserName = Constants.EXISTING_USERNAME };
            var roles = new List<string> { Constants.TEST_ROLE1, Constants.TEST_ROLE2 };
            var dto = new LoginDto { Username = Constants.EXISTING_USERNAME, Password = Constants.VALID_PASSWORD };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(roles);

            var result = await _identityService.ValidateUserCredentialsAndGetRolesAsync(dto);

            result.user.Should().Be(user);
            result.roles.Should().BeEquivalentTo(roles);
        }


        [Fact]
        public async Task CreateUserAsync_WhenUsernameAlreadyExists_ShouldThrowConflictException()
        {
            var dto = new RegisterDto { Username = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(new MyIdentityUser());

            await FluentActions.Invoking(() => _identityService.CreateUserAsync(dto))
                .Should().ThrowAsync<ConflictException>()
                .WithMessage(ErrorMessages.UsernameAlreadyExists);
        }

        [Fact]
        public async Task CreateUserAsync_WhenCreateFails_ShouldThrowOperationFailedException()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((MyIdentityUser?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<MyIdentityUser>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            await FluentActions.Invoking(() => _identityService.CreateUserAsync(dto))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task CreateUserAsync_WhenSuccessful_ShouldReturnUser()
        {
            var dto = new RegisterDto { Username = Constants.NEW_USERNAME, Password = Constants.VALID_PASSWORD };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((MyIdentityUser?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<MyIdentityUser>(), dto.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _identityService.CreateUserAsync(dto);

            result.Should().NotBeNull();
            result.UserName.Should().Be(dto.Username);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            var dto = new UpdateUserRoleDto { Username = Constants.NONEXISTENT_USERNAME, RoleName = Constants.TEST_ROLE1, Add = true };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((MyIdentityUser?)null);

            await FluentActions.Invoking(() => _identityService.UpdateUserRoleAsync(dto))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage(ErrorMessages.UserNotFound);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_WhenRoleDoesNotExist_ShouldThrowNotFoundException()
        {
            var dto = new UpdateUserRoleDto { Username = Constants.EXISTING_USERNAME, RoleName = Constants.TEST_ROLE1, Add = true };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(new MyIdentityUser());
            _roleManagerMock.Setup(r => r.RoleExistsAsync(dto.RoleName)).ReturnsAsync(false);

            await FluentActions.Invoking(() => _identityService.UpdateUserRoleAsync(dto))
                .Should().ThrowAsync<NotFoundException>()
                .WithMessage(ErrorMessages.RoleDoesNotExist(dto.RoleName));
        }

        [Fact]
        public async Task UpdateUserRoleAsync_WhenAddOrRemoveFails_ShouldThrowOperationFailedException()
        {
            var dto = new UpdateUserRoleDto { Username = Constants.EXISTING_USERNAME, RoleName = Constants.TEST_ROLE1, Add = true };
            var user = new MyIdentityUser { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1))
                            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = Constants.ERROR_MESSAGE }));

            await FluentActions.Invoking(() => _identityService.UpdateUserRoleAsync(dto))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task UpdateUserRole_WhenAddOrRemoveSucceeds_ShouldNotThrow()
        {
            var dto = new UpdateUserRoleDto { Username = Constants.EXISTING_USERNAME, RoleName = Constants.TEST_ROLE1, Add = true };
            var user = new MyIdentityUser { UserName = Constants.EXISTING_USERNAME };
            _userManagerMock.Setup(u => u.FindByNameAsync(Constants.EXISTING_USERNAME)).ReturnsAsync(user);
            _roleManagerMock.Setup(r => r.RoleExistsAsync(Constants.TEST_ROLE1)).ReturnsAsync(true);
            _userManagerMock.Setup(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1)).ReturnsAsync(IdentityResult.Success);

             await _identityService.UpdateUserRoleAsync(dto);

            _userManagerMock.Verify(u => u.AddToRoleAsync(user, Constants.TEST_ROLE1), Times.Once);
        }

        [Fact]
        public async Task CreateRoleAsync_WithEmptyRole_ShouldThrowValidationException()
        {
            await FluentActions.Invoking(() => _identityService.CreateRoleAsync(string.Empty))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage(ErrorMessages.RoleNameCannotBeEmpty);
        }

        [Fact]
        public async Task CreateRoleAsync_WhenRoleAlreadyExists_ShouldThrowConflictException()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(true);

            await FluentActions.Invoking(() => _identityService.CreateRoleAsync(Constants.TEST_ROLE1))
                .Should().ThrowAsync<ConflictException>()
                .WithMessage(ErrorMessages.RoleAlreadyExists);
        }

        [Fact]
        public async Task CreateRoleAsync_WhenRoleCreationFails_ShouldThrowOperationFailedException()
        {
            var identityErrors = new IdentityError[] { new IdentityError { Description = Constants.ERROR_MESSAGE } };
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            await FluentActions.Invoking(() => _identityService.CreateRoleAsync(Constants.TEST_ROLE1))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task CreateRoleAsync_WhenRoleCreationSucceeds_ShouldReturnRole()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _identityService.CreateRoleAsync(Constants.TEST_ROLE1);

            result.Should().Be(Constants.TEST_ROLE1);
        }

        [Fact]
        public async Task GetAllRolesAsync_ShouldReturnAllRoles()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole(Constants.TEST_ROLE1),
                new IdentityRole(Constants.TEST_ROLE2)
            }.AsQueryable();

            _roleManagerMock.Setup(r => r.Roles).Returns(roles);

            var result = await _identityService.GetAllRolesAsync();

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(new[]
            {
                Constants.TEST_ROLE1,
                Constants.TEST_ROLE2
            });
        }

        [Fact]
        public async Task GetAllRolesAsync_WhenNoRoles_ShouldReturnEmpty()
        {
            var roles = new List<IdentityRole>().AsQueryable();

            _roleManagerMock.Setup(r => r.Roles).Returns(roles);

            var result = await _identityService.GetAllRolesAsync();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
