using AuthService.Api.Extensions;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// Add configurations to the container
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Services
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithFilters();

// Swagger
builder.Services.AddSwagger();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUIEndpoints();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();