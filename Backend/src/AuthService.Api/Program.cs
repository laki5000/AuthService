using AuthService.Api.Extensions;
using AuthService.Application.Constants;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add configurations to the container
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(CommonConstants.JwtSectionName));

// Services
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithFilters();

// Swagger
builder.Services.AddSwagger();

// Cors
var allowedOrigins = builder.Configuration.GetSection(CommonConstants.AllowedOriginsSectionName).Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy(CommonConstants.CorsPolicyName, policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUIEndpoints();
}

app.UseHttpsRedirection();

app.UseCors(CommonConstants.CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();