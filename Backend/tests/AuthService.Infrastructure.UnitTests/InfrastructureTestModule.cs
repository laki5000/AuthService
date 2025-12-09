using AuthService.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AuthService.Infrastructure.UnitTests
{
    public class InfrastructureTestModule
    {
        public JwtOptions JwtOptions { get; }
        public TokenService TokenService { get; }

        public InfrastructureTestModule()
        {
            JwtOptions = new JwtOptions
            {
                Key = Constants.KEY,
                Issuer = Constants.ISSUER,
                Audience = Constants.AUDIENCE,
                ExpiresMinutes = Constants.EXPIRES_MINUTES
            };

            var services = new ServiceCollection();

            services.AddSingleton(Options.Create(JwtOptions));
            services.AddTransient<TokenService>();

            var provider = services.BuildServiceProvider();
            TokenService = provider.GetRequiredService<TokenService>();
        }
    }
}
