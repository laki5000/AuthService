using Microsoft.AspNetCore.Http;

namespace AuthService.Domain.DTOs
{
    public class ResultDto<T>
    {
        public bool Success { get; set; } = true;
        public T? Result { get; set; }
        public string? Error { get; set; }
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
    }
}
