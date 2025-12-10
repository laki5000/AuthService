using AuthService.Application.Constants;
using Microsoft.AspNetCore.Http;

namespace AuthService.Application.Helpers
{
    public static class CookieHelper
    {
        public static void SetJwtCookie(HttpResponse response, string token, int expiresMinutes)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(expiresMinutes)
            };

            response.Cookies.Append(CommonConstants.JwtCookieName, token, cookieOptions);
        }

        public static void DeleteJwtCookie(HttpRequest request, HttpResponse response)
        {
            if (request.Cookies.ContainsKey(CommonConstants.JwtCookieName))
            {
                response.Cookies.Delete(CommonConstants.JwtCookieName);
            }
        }
    }
}
