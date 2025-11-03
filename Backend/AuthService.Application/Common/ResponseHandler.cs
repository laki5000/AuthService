using AuthService.Application.Interfaces.Common;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AuthService.Application.Common
{
    public class ResponseHandler : IResponseHandler
    {
        public static readonly string UnauthorizedMessage = "Login failed or token is invalid.";
        public static readonly string ForbiddenMessage = "You do not have the required permissions.";

        public (bool handled, string? jsonResponse, int? newStatusCode) HandleResponse(
            int statusCode,
            string? existingBody)
        {
            if ((statusCode == StatusCodes.Status401Unauthorized || statusCode == StatusCodes.Status403Forbidden)
                && string.IsNullOrEmpty(existingBody))
            {
                var result = new ResultDto<string>
                {
                    Success = false,
                    Errors = new[]
                    {
                        statusCode == StatusCodes.Status401Unauthorized
                            ? UnauthorizedMessage
                            : ForbiddenMessage
                    },
                    StatusCode = statusCode
                };

                var json = JsonSerializer.Serialize(result);
                return (handled: true, jsonResponse: json, newStatusCode: statusCode);
            }

            if (!string.IsNullOrEmpty(existingBody))
            {
                try
                {
                    var resultDto = JsonSerializer.Deserialize<ResultDto<object>>(existingBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (resultDto?.StatusCode is int innerCode)
                        return (handled: false, jsonResponse: existingBody, newStatusCode: innerCode);
                }
                catch (Exception ex)
                {
                    _ = ex; // intentionally ignored
                }
            }

            return (handled: false, jsonResponse: existingBody, newStatusCode: null);
        }
    }
}
