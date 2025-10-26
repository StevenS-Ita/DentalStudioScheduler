using DentalStudioScheduler.Context;
using DentalStudioScheduler.Services;
using Microsoft.EntityFrameworkCore;

namespace DentalStudioScheduler.Extensions
{
    public static class ConfigureExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registra il contesto EF
            services.AddDbContext<DentalStudioContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registra i servizi custom
            services.AddScoped<AppointmentService>();

            return services;
        }
    }
}
