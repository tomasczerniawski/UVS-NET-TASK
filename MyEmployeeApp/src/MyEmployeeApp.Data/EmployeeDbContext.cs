using Microsoft.EntityFrameworkCore;
using MyEmployeeApp.Core;

namespace MyEmployeeApp.Data
{
    public interface IEmployeeDbContext
    {
        DbSet<Employee> Employees { get; set; }
        int SaveChanges();
    }

    public class EmployeeDbContext : DbContext, IEmployeeDbContext
    {
        public EmployeeDbContext()
        {
        }
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Server=localhost;Port=7777;User ID=postgres;Password=guest;Database=uvsproject;");
            }
        }
    }
}
