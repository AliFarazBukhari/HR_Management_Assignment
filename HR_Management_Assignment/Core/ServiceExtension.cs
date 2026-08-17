using Microsoft.EntityFrameworkCore;
using Repository;
using Services;

namespace HR_Management_Assignment.Core
{
    public static class ServiceExtension
    {
        public static IServiceCollection ConfigureDependecies(this IServiceCollection service, ConfigurationManager configuration)
        {
            service.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200") // Angular dev server URL
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });


            service.AddDbContext<HrDbContext>(options =>
             {
                 options.UseSqlServer(
                     configuration.GetConnectionString("DefaultConnection"));
             });


            service.AddScoped<HRRepository>();
            service.AddScoped<LeaveService>();

            return service;
        }
    }
}
