using AuthService.Application.Interfaces.Services;
using AuthService.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace AuthService.Application.UnitTests
{
    public class ApplicationTestModule
    {
        public IServiceProvider Provider { get; }

        public Mock<ITokenService> TokenServiceMock { get; }
        public Mock<IIdentityService> IdentityServiceMock { get; }

        public ApplicationTestModule()
        {
            var services = new ServiceCollection();

            TokenServiceMock = new Mock<ITokenService>();
            IdentityServiceMock = new Mock<IIdentityService>();

            services.AddSingleton(TokenServiceMock.Object);
            services.AddSingleton(IdentityServiceMock.Object);

            services.AddScoped<MyAuthService>();

            Provider = services.BuildServiceProvider();
        }

        public T GetScopedService<T>() where T : class
        {
            using var scope = Provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<T>();
        }
    }
}
