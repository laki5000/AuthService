namespace AuthService.Application.Interfaces.Common
{
    public interface IResponseHandler
    {
        (bool handled, string? jsonResponse, int? newStatusCode) HandleResponse(
            int statusCode,
            string? existingBody
        );
    }
}
