namespace AuthService.Domain.Entities
{
    public interface IUserFactory
    {
        IUser Create(string? username = null, string? email = null, string? firstName = null, string? lastName = null);
    }

}
