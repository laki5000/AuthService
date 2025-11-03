using AuthService.Application.Common;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace AuthService.Application.UnitTests.Common
{
    public class ResponseHandlerTests
    {
        private readonly ResponseHandler _responseHandler;

        public ResponseHandlerTests() { 
            _responseHandler = new ResponseHandler();
        }

        [Fact]
        public void HandleResponse_WithStatus401AndEmptyBody_ShouldReturnUnauthorized()
        {
            var (handled, jsonResponse, newStatusCode) = _responseHandler.HandleResponse(StatusCodes.Status401Unauthorized, null);

            Assert.True(handled);
            Assert.Equal(StatusCodes.Status401Unauthorized, newStatusCode);
            var dto = JsonSerializer.Deserialize<ResultDto<string>>(jsonResponse!);
            Assert.False(dto!.Success);
            Assert.Contains(ResponseHandler.UnauthorizedMessage, dto.Errors!);
        }

        [Fact]
        public void HandleResponse_WithStatus403AndEmptyBody_ShouldReturnForbidden()
        {
            var (handled, jsonResponse, newStatusCode) = _responseHandler.HandleResponse(StatusCodes.Status403Forbidden, null);

            Assert.True(handled);
            Assert.Equal(StatusCodes.Status403Forbidden, newStatusCode);
            var dto = JsonSerializer.Deserialize<ResultDto<string>>(jsonResponse!);
            Assert.False(dto!.Success);
            Assert.Contains(ResponseHandler.ForbiddenMessage, dto.Errors!);
        }

        [Fact]
        public void HandleResponse_WithExistingValidJson_ShouldPreserveBodyAndStatusCode()
        {
            var originalDto = new ResultDto<object> { Success = true, StatusCode = StatusCodes.Status200OK };
            var body = JsonSerializer.Serialize(originalDto);

            var (handled, jsonResponse, newStatusCode) = _responseHandler.HandleResponse(StatusCodes.Status200OK, body);

            Assert.False(handled);
            Assert.Equal(body, jsonResponse);
            Assert.Equal(StatusCodes.Status200OK, newStatusCode);
        }

        [Fact]
        public void HandleResponse_WithInvalidJson_ShouldReturnOriginalBodyAndNullStatusCode()
        {
            var invalidJson = Constants.NOT_VALID_JSON;

            var (handled, jsonResponse, newStatusCode) = _responseHandler.HandleResponse(StatusCodes.Status500InternalServerError, invalidJson);

            Assert.False(handled);
            Assert.Equal(invalidJson, jsonResponse);
            Assert.Null(newStatusCode);
        }
    }
}
