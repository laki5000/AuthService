using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity
{
    public static class IdentityConfig
    {
        public static void Configure(IdentityOptions options)
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;

            options.User.RequireUniqueEmail = true;
        }
    }
}
