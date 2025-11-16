using AuthService.Application.Constants;
using AuthService.Application.Exceptions;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Api.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            int statusCode;
            object responseBody;

            switch (context.Exception)
            {
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = validationEx.Message,
                        StatusCode = statusCode
                    };
                    break;
                case AuthenticationException authEx:
                    statusCode = StatusCodes.Status401Unauthorized;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = authEx.Message,
                        StatusCode = statusCode
                    };
                    break;
                case ConflictException conflictEx:
                    statusCode = StatusCodes.Status409Conflict;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = conflictEx.Message,
                        StatusCode = statusCode
                    };
                    break;
                case NotFoundException notFoundEx:
                    statusCode = StatusCodes.Status404NotFound;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = notFoundEx.Message,
                        StatusCode = statusCode
                    };
                    break;
                case OperationFailedException opFailEx:
                    statusCode = StatusCodes.Status500InternalServerError;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = opFailEx.Message,
                        StatusCode = statusCode
                    };
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    responseBody = new ResultDto<string>
                    {
                        Success = false,
                        Error = ErrorMessages.AnUnexpectedErrorOccurred,
                        StatusCode = statusCode
                    };
                    break;
            }

            context.Result = new ObjectResult(responseBody)
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}
