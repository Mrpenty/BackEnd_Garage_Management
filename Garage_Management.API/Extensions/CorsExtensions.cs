namespace Garage_Management.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7160")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            return services;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseCors("DefaultCorsPolicy");
            }
            else
            {
                app.UseCors("DefaultCorsPolicy");
            }

            return app;
        }
    }
}

