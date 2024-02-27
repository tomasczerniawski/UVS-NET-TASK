using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyEmployeeApp.Data;
using MyEmployeeApp.Core;

namespace MyEmployeeApp.ConsoleApp.Configuration
{
    public static class ServiceConfig
    {
        public static void ConfigureServices(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IEmployeeDbContext, EmployeeDbContext>(options =>
                options.UseNpgsql(connectionString)
            );
        }
    }
}