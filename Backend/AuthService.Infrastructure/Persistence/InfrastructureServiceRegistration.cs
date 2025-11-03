using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Persistence
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            var conn = config.GetConnectionString("Postgres");
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseNpgsql(conn));

            services.AddIdentityCore<User>(options =>
            {
                IdentityConfig.Configure(options);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<User>>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<ITokenHeaderService, TokenHeaderService>();

            return services;
        }
    }
}
