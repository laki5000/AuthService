namespace AuthService.Domain.DTOs
{
    public class UpdateUserRoleDto : RoleDto
    {
        public string Username { get; set; } = default!;
        public bool Add { get; set; } = true;
    }
}
