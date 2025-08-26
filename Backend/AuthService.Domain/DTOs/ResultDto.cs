using System.Net;

namespace AuthService.Domain.DTOs
{
    public class ResultDto<T>
    {
        public bool Success { get; set; }
        public T? Result { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public int StatusCode { get; set; } = (int)HttpStatusCode.OK;
    }
}
