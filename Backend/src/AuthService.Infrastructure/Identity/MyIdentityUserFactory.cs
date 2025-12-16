using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Identity
{
    public class MyIdentityUserFactory : IUserFactory
    {
        public IUser Create(string? username = null, string? email = null, string? firstName = null, string? lastName = null)
        {
            return new MyIdentityUser
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
}
