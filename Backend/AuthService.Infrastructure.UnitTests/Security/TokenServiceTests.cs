using AuthService.Domain.Entities;
using AuthService.Infrastructure.Security;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Infrastructure.UnitTests.Security
{
    public class TokenServiceTests
    {
        private readonly JwtOptions _jwtOptionsMock;

        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _jwtOptionsMock = new JwtOptions
            {
                Key = Constants.KEY,
                Issuer = Constants.ISSUER,
                Audience = Constants.AUDIENCE,
                ExpiresMinutes = Constants.EXPIRES_MINUTES,
            };

            var optionsMock = Options.Create(_jwtOptionsMock);
            _tokenService = new TokenService(optionsMock);
        }

        [Fact]
        public void GenerateToken_WhenSuccessful_ShouldReturnValidToken()
        {
            var user = new User
            {
                Id = Constants.ID,
                UserName = Constants.USERNAME,
                Email = Constants.EMAIL
            };
            var roles = new List<string> { Constants.ROLE1 ,Constants.ROLE2 };

            var tokenString = _tokenService.GenerateToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == user.UserName);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == Constants.ROLE1);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == Constants.ROLE2);

            var expectedExpiry = DateTime.UtcNow.AddMinutes(_jwtOptionsMock.ExpiresMinutes);
            Assert.True((token.ValidTo - expectedExpiry).TotalSeconds < 5);
        }
    }
}
