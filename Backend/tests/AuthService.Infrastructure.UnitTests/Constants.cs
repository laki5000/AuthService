namespace AuthService.Infrastructure.UnitTests
{
    public static class Constants
    {
        public const string EXISTING_USERNAME = "existing";
        public const string NEW_USERNAME = "newuser";
        public const string NONEXISTENT_USERNAME = "nonexistent";
        public const string VALID_PASSWORD = "correctpass";
        public const string INVALID_PASSWORD = "wrongpass";
        public const string TEST_ROLE1 = "Admin";
        public const string TEST_ROLE2 = "User";
        public const string NONEXISTENT_ROLE = "NonexistentRole";
        public const string JWT_TOKEN = "jwt-token";
        public const string ERROR_MESSAGE = "Error";
        public const string NOT_VALID_JSON = "{ not valid json }";

        public const string KEY = "!!!TEST_ONLY_CHANGE_ME_LONG_RANDOM_SECRET!!!";
        public const string ISSUER = "MyIssuer";
        public const string AUDIENCE = "MyAudience";
        public const int EXPIRES_MINUTES = 60;
        public const string ID = "123";
        public const string EMAIL = "e@mail.com";
    }
}
