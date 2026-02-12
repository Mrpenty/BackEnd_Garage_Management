using Garage_Management.Base.Interface;
using Garage_Management.Base.Services;

namespace Garage_Management.API.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencyInjectionServices(this IServiceCollection services)
        {
            services.AddScoped<ISmsService, TwilioSmsService>();
            services.AddScoped<IGenerateToken, GenerateToken>();

            return services;
        }
    }
}
