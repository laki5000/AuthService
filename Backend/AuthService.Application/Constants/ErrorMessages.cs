namespace AuthService.Application.Constants
{
    public static class ErrorMessages
    {
        public const string RoleNameCannotBeEmpty = "Role name cannot be empty";
        public const string RoleAlreadyExists = "Role already exists";
        public const string InvalidCredentials = "Invalid credentials";
        public const string UsernameAlreadyExists = "Username already exists";
        public const string UserNotFound = "User not found";
        public static string RoleDoesNotExist(string role) => $"Role '{role}' does not exist.";

    }
}
