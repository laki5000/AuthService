using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Infrastructure.Identity
{
    public class MyIdentityUser : IdentityUser, IUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
