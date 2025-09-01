using AuthService.Application.Constants;
using AuthService.Application.Services;
using AuthService.Application.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthService.Application.UnitTests
{
    public class RoleServiceTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            _roleService = new RoleService(_roleManagerMock.Object);
        }

        [Fact]
        public async Task CreateAsync_WithEmptyRole_ShouldReturnError()
        {
            var result = await _roleService.CreateAsync("");

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.RoleNameCannotBeEmpty);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleAlreadyExists_ShouldReturnError()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(true);

            var result = await _roleService.CreateAsync("Admin");

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(ErrorMessages.RoleAlreadyExists);
        }

        [Fact]
        public async Task CreateAsync_WhenRoleCreationFails_ShouldReturnIdentityErrors()
        {
            var identityErrors = new IdentityError[] { new IdentityError { Description = "Some error" } };
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Failed(identityErrors));

            var result = await _roleService.CreateAsync("Admin");

            result.Success.Should().BeFalse();
            result.Errors.Should().Contain("Some error");
        }

        [Fact]
        public async Task CreateAsync_WhenRoleCreationSucceeds_ShouldReturnSuccess()
        {
            _roleManagerMock.Setup(x => x.RoleExistsAsync("Admin"))
                .ReturnsAsync(false);
            _roleManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _roleService.CreateAsync("Admin");

            result.Success.Should().BeTrue();
            result.Result.Should().Be("Admin");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllRoles()
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole("Admin"),
                new IdentityRole("User")
            }.AsQueryable();
            var asyncRoles = new TestAsyncEnumerable<IdentityRole>(roles);

            _roleManagerMock.Setup(r => r.Roles).Returns(asyncRoles);

            var result = await _roleService.GetAllAsync();

            result.Success.Should().BeTrue();
            result.Result.Should().HaveCount(2);
            result.Result.Should().Contain(new[] { "Admin", "User" });
        }

        [Fact]
        public async Task GetAllAsync_WhenNoRoles_ShouldReturnEmpty()
        {
            var roles = new List<IdentityRole>()
                .AsQueryable();
            var asyncRoles = new TestAsyncEnumerable<IdentityRole>(roles);

            _roleManagerMock.Setup(r => r.Roles).Returns(asyncRoles);

            var result = await _roleService.GetAllAsync();

            result.Success.Should().BeTrue();
            result.Result.Should().BeEmpty();
        }
    }
}