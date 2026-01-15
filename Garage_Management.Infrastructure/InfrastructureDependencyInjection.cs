using Garage_Management.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;    
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Garage_Management.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services, string connectionString)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(InfrastructureDependencyInjection))
                .AddClasses(classes => classes.Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);

            });

            return services;
        }
    }
}
    