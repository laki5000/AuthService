namespace AuthService.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUIEndpoints(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
