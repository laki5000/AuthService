using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Application.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace AuthService.Application.UnitTests.Services
{
    public class RoleServiceTests
    {
        private readonly RoleService _roleService;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

        public RoleServiceTests()
        {
            var module = new ApplicationTestModule();

            _roleService = module.RoleService;
            _roleManagerMock = module.RoleManagerMock;
        }

        [Fact]
        public async Task CreateAsync_WithEmptyRole_ShouldThrowValidationException()
        {
            await FluentActions.Invoking(() => _roleService.CreateAsync(string.Empty))
                .Should().ThrowAsync<ValidationException>()
                .WithMessage(ErrorMessages.RoleNameCannotBeEmpty);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleAlreadyExists_ShouldThrowConflictException()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(true);

            await FluentActions.Invoking(() => _roleService.CreateAsync(Constants.TEST_ROLE1))
                .Should().ThrowAsync<ConflictException>()
                .WithMessage(ErrorMessages.RoleAlreadyExists);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleCreationFails_ShouldThrowOperationFailedException()
        {
            var identityErrors = new IdentityError[] { new IdentityError { Description = Constants.ERROR_MESSAGE } };
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            await FluentActions.Invoking(() => _roleService.CreateAsync(Constants.TEST_ROLE1))
                .Should().ThrowAsync<OperationFailedException>()
                .WithMessage(Constants.ERROR_MESSAGE);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleCreationSucceeds_ShouldReturnSuccess()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync(Constants.TEST_ROLE1))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _roleService.CreateAsync(Constants.TEST_ROLE1);

            result.Success.Should().BeTrue();
            result.Result.Should().Be(Constants.TEST_ROLE1);
        }

        [Fact]
        public void GetAll_ShouldReturnAllRoles()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole(Constants.TEST_ROLE1),
                new IdentityRole(Constants.TEST_ROLE2)
            }.AsQueryable();

            _roleManagerMock.Setup(r => r.Roles).Returns(roles);

            var result = _roleService.GetAll();

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain([Constants.TEST_ROLE1, Constants.TEST_ROLE2]);
        }

        [Fact]
        public void GetAll_WhenNoRoles_ShouldReturnEmpty()
        {
            var roles = new List<IdentityRole>()
                .AsQueryable();

            _roleManagerMock.Setup(r => r.Roles).Returns(roles);

            var result = _roleService.GetAll();

            result.Success.Should().BeTrue();
            result.Result.Should().BeEmpty();
        }
    }
}