namespace AuthService.Domain.Entities
{
    public interface IUser
    {
        string Id { get; set; }
        string? UserName { get; set; }
        string? Email { get; set; }
        string? FirstName { get; set; }
        string? LastName { get; set; }
    }
}
