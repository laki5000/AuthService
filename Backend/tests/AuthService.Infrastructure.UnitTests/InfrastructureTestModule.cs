using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AuthService.Infrastructure.UnitTests
{
    public class InfrastructureTestModule
    {
        public IServiceProvider Provider { get; }

        public Mock<UserManager<MyIdentityUser>> UserManagerMock { get; }
        public Mock<RoleManager<IdentityRole>> RoleManagerMock { get; }
        public Mock<SignInManager<MyIdentityUser>> SignInManagerMock { get; }
        public Mock<ITokenService> TokenServiceMock { get; }

        public InfrastructureTestModule()
        {
            var services = new ServiceCollection();

            var jwtOptions = new JwtOptions
            {
                Key = Constants.KEY,
                Issuer = Constants.ISSUER,
                Audience = Constants.AUDIENCE,
                ExpiresMinutes = Constants.EXPIRES_MINUTES
            };
            services.AddSingleton(Options.Create(jwtOptions));

            UserManagerMock = CreateUserManagerMock();
            RoleManagerMock = CreateRoleManagerMock();
            SignInManagerMock = CreateSignInManagerMock(UserManagerMock.Object);
            TokenServiceMock = new Mock<ITokenService>();

            services.AddSingleton(UserManagerMock.Object);
            services.AddSingleton(RoleManagerMock.Object);
            services.AddSingleton(SignInManagerMock.Object);
            services.AddSingleton(TokenServiceMock.Object);

            services.AddScoped<JwtTokenService>();
            services.AddScoped<IdentityAuthService>();

            Provider = services.BuildServiceProvider();
        }

        public T GetScopedService<T>() where T : class
        {
            using var scope = Provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }

        private static Mock<UserManager<MyIdentityUser>> CreateUserManagerMock()
        {
            return new Mock<UserManager<MyIdentityUser>>(
                new Mock<IUserStore<MyIdentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<MyIdentityUser>>().Object,
                new IUserValidator<MyIdentityUser>[] { new Mock<IUserValidator<MyIdentityUser>>().Object },
                new IPasswordValidator<MyIdentityUser>[] { new Mock<IPasswordValidator<MyIdentityUser>>().Object },
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<MyIdentityUser>>>().Object
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

        private static Mock<SignInManager<MyIdentityUser>> CreateSignInManagerMock(UserManager<MyIdentityUser> userManager)
        {
            return new Mock<SignInManager<MyIdentityUser>>(
                userManager,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<MyIdentityUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<MyIdentityUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object
            );
        }
    }
}
