using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyEmployeeApp.Data;
using MyEmployeeApp.Core;

namespace MyEmployeeApp.ConsoleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();

                ConfigureServices(services);

                var serviceProvider = services.BuildServiceProvider();

                if (args.Length < 1)
                {
                    Console.WriteLine("No command provided.");
                    return;
                }

                var command = args[0].ToLower();

                Console.WriteLine($"Command: {command}");
                Console.WriteLine($"Arguments: {string.Join(", ", args)}");

                using var scope = serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

                switch (command)
                {
                    case "get-employee":
                        HandleGetEmployeeCommand(args, dbContext);
                        break;

                    case "set-employee":
                        HandleSetEmployeeCommand(args, dbContext);
                        break;

                    default:
                        Console.WriteLine($"Invalid command: {command}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public static void HandleGetEmployeeCommand(string[] args, IEmployeeDbContext dbContext)
        {
            try
            {
                if (args.Length != 3 || args[1] != "--employeeId" || !int.TryParse(args[2], out var employeeId))
                {
                    Console.WriteLine("Invalid or missing arguments for get-employee.");
                    Console.WriteLine("Usage: get-employee --employeeId <id>");
                    return;
                }

                var employee = dbContext.Employees?.Find(employeeId);
                if (employee != null)
                {
                    Console.WriteLine($"Employee found: {employee.EmployeeName}, Salary: {employee.EmployeeSalary}");
                }
                else
                {
                    Console.WriteLine($"Employee with ID {employeeId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving employee data: {ex.Message}");
            }
        }


        public static void HandleSetEmployeeCommand(string[] args, IEmployeeDbContext dbContext)
        {

            if (!int.TryParse(args[2], out var newEmployeeId) || !int.TryParse(args[6], out var newEmployeeSalary))
            {
                Console.WriteLine("Invalid arguments for set-employee. EmployeeId and EmployeeSalary must be integers.");
                return;
            }

            var newEmployeeName = args[4];

            var existingEmployee = dbContext.Employees?.Find(newEmployeeId);
            if (existingEmployee != null)
            {
                Console.WriteLine($"Employee with ID {newEmployeeId} already exists. Cannot add duplicate entries.");
                return;
            }

            if (newEmployeeSalary < 0)
            {
                Console.WriteLine("Employee salary cannot be negative.");
                return;
            }

            if (newEmployeeName.Length > 15)
            {
                Console.WriteLine("Employee name exceeds maximum length.");
                return;
            }

            var newEmployee = new Employee { EmployeeId = newEmployeeId, EmployeeName = newEmployeeName, EmployeeSalary = newEmployeeSalary };
            if (newEmployee != null)
            {
                dbContext.Employees?.Add(newEmployee);
                dbContext.SaveChanges();
                Console.WriteLine($"Employee added: {newEmployeeName}, Salary: {newEmployeeSalary}");
            }
            else
            {
                Console.WriteLine("Invalid employee object.");
                throw new ArgumentNullException(nameof(newEmployee), "Employee object is null.");
            }
        }

        static void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Server=localhost;Port=7777;User ID=postgres;Password=guest;Database=uvsproject;";

            services.AddDbContext<EmployeeDbContext>(options =>
                options.UseNpgsql(connectionString)
            );

            services.AddScoped<IEmployeeDbContext, EmployeeDbContext>();
        }
    }
}