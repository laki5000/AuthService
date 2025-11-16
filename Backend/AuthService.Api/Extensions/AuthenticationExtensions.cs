using AuthService.Application.Constants;
using AuthService.Domain.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace AuthService.Api.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureJwt(this IServiceCollection services, IConfiguration config)
        {
            var jwt = config.GetSection("Jwt");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwt["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwt["Audience"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = CommonConstants.ApplicationJson;
                            var result = JsonSerializer.Serialize(new ResultDto<string>
                            {
                                Success = false,
                                Error = ErrorMessages.UnauthorizedMessage,
                                StatusCode = StatusCodes.Status401Unauthorized
                            });
                            return context.Response.WriteAsync(result);
                        },
                        OnForbidden = context =>
                        {
                            context.Response.ContentType = CommonConstants.ApplicationJson;
                            var result = JsonSerializer.Serialize(new ResultDto<string>
                            {
                                Success = false,
                                Error = ErrorMessages.ForbiddenMessage,
                                StatusCode = StatusCodes.Status403Forbidden
                            });
                            return context.Response.WriteAsync(result);
                        },
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(CommonConstants.JwtCookieName, out var token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
