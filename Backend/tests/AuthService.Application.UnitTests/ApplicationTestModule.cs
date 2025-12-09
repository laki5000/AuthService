using AuthService.Application.Interfaces.Services;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthService.Application.UnitTests
{
    public class ApplicationTestModule
    {
        public Mock<UserManager<User>> UserManagerMock { get; }
        public Mock<RoleManager<IdentityRole>> RoleManagerMock { get; }
        public Mock<SignInManager<User>> SignInManagerMock { get; }
        public Mock<ITokenService> TokenServiceMock { get; }

        public UserService UserService { get; }
        public RoleService RoleService { get; }

        public ApplicationTestModule()
        {
            UserManagerMock = CreateUserManagerMock();
            RoleManagerMock = CreateRoleManagerMock();
            SignInManagerMock = CreateSignInManagerMock(UserManagerMock.Object);
            TokenServiceMock = new Mock<ITokenService>();

            var services = new ServiceCollection();

            services.AddSingleton(UserManagerMock.Object);
            services.AddSingleton(RoleManagerMock.Object);
            services.AddSingleton(SignInManagerMock.Object);
            services.AddSingleton(TokenServiceMock.Object);
            services.AddTransient<UserService>();
            services.AddTransient<RoleService>();

            var provider = services.BuildServiceProvider();

            UserService = provider.GetRequiredService<UserService>();
            RoleService = provider.GetRequiredService<RoleService>();
        }

        private static Mock<UserManager<User>> CreateUserManagerMock()
        {
            return new Mock<UserManager<User>>(
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
        }

        private static Mock<RoleManager<IdentityRole>> CreateRoleManagerMock()
        {
            return new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object,
                new List<IRoleValidator<IdentityRole>> { new Mock<IRoleValidator<IdentityRole>>().Object },
                new Mock<ILookupNormalizer>().Object,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object
            );
        }

        private static Mock<SignInManager<User>> CreateSignInManagerMock(UserManager<User> userManager)
        {
            return new Mock<SignInManager<User>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<User>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object
            );
        }
    }
}
