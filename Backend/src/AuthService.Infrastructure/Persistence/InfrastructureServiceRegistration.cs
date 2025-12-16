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

            services.AddIdentityCore<MyIdentityUser>(options =>
            {
                IdentityConfig.Configure(options);
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager<SignInManager<MyIdentityUser>>();

            services.AddScoped<ITokenService, JwtTokenService>();

            return services;
        }
    }
}
