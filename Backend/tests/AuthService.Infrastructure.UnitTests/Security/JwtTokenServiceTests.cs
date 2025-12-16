using AuthService.Infrastructure.Identity;
using AuthService.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthService.Infrastructure.UnitTests.Security
{
    public class JwtTokenServiceTests
    {
        private readonly JwtTokenService _tokenService;
        private readonly JwtOptions _jwtOptionsMock;

        public JwtTokenServiceTests()
        {
            var module = new InfrastructureTestModule();

            _tokenService = module.GetScopedService<JwtTokenService>();
            _jwtOptionsMock = module.Provider.GetRequiredService<IOptions<JwtOptions>>().Value;
        }

        [Fact]
        public void GenerateToken_WhenSuccessful_ShouldReturnValidToken()
        {
            var user = new MyIdentityUser
            {
                Id = Constants.ID,
                UserName = Constants.NEW_USERNAME,
                Email = Constants.EMAIL
            };
            var roles = new List<string> { Constants.TEST_ROLE1, Constants.TEST_ROLE2 };

            var tokenString = _tokenService.GenerateToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == user.UserName);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == Constants.TEST_ROLE1);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Role && c.Value == Constants.TEST_ROLE2);

            var expectedExpiry = DateTime.UtcNow.AddMinutes(_jwtOptionsMock.ExpiresMinutes);
            Assert.True((token.ValidTo - expectedExpiry).TotalSeconds < 5);
        }
    }
}
