using Microsoft.IdentityModel.Tokens.Experimental;

namespace AuthService.Infrastructure.UnitTests
{
    public static class Constants
    {
        public const string KEY = "!!!TEST_ONLY_CHANGE_ME_LONG_RANDOM_SECRET!!!";
        public const string ISSUER = "MyIssuer";
        public const string AUDIENCE = "MyAudience";
        public const int EXPIRES_MINUTES = 60;
        public const string ID = "123";
        public const string USERNAME = "username";
        public const string EMAIL = "e@mail.com";
        public const string ROLE1 = "Admin";
        public const string ROLE2 = "User";
    }
}
