using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace AuthService.Infrastructure.UnitTests.Security
{
    public class TokenHeaderServiceTests
    {
        private readonly TokenHeaderService _tokenHeaderService;

        public TokenHeaderServiceTests() {
            _tokenHeaderService = new TokenHeaderService();
        }

        [Fact]
        public void AddJwtFromCookieToHeader_WhenCookieExists_ShouldAddAuthorizationHeader()
        {
            var context = new DefaultHttpContext();
            context.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                { TokenHeaderService.JwtCookieName, Constants.JWT_TOKEN }
            });

            _tokenHeaderService.AddJwtFromCookieToHeader(context);

            Assert.True(context.Request.Headers.ContainsKey(TokenHeaderService.AuthorizationHeaderName));
            Assert.Equal($"{TokenHeaderService.BearerPrefix}{Constants.JWT_TOKEN}", context.Request.Headers[TokenHeaderService.AuthorizationHeaderName]);
        }

        [Fact]
        public void AddJwtFromCookieToHeader_WhenCookieMissing_ShouldNotAddAuthorizationHeader()
        {
            var context = new DefaultHttpContext();
            context.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>());

            _tokenHeaderService.AddJwtFromCookieToHeader(context);

            Assert.False(context.Request.Headers.ContainsKey(TokenHeaderService.AuthorizationHeaderName));
        }

        [Fact]
        public void AddJwtFromCookieToHeader_WhenHeaderAlreadyExists_ShouldNotOverwrite()
        {
            var context = new DefaultHttpContext();
            context.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                { TokenHeaderService.JwtCookieName, Constants.JWT_TOKEN }
            });
            context.Request.Headers[TokenHeaderService.AuthorizationHeaderName] = Constants.EXISTING_HEADER_VALUE;

            _tokenHeaderService.AddJwtFromCookieToHeader(context);

            Assert.Equal(Constants.EXISTING_HEADER_VALUE, context.Request.Headers[TokenHeaderService.AuthorizationHeaderName]);
        }
    }
}
