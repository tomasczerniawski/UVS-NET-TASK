using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace DatabaseSchema
{
    class Program
    {
        private static readonly string Password = Environment.GetEnvironmentVariable("UvsTaskPassword")
            ?? throw new InvalidOperationException("You must set the UvsTaskPassword environment variable");
        private static readonly string Database = Environment.GetEnvironmentVariable("UvsTaskDatabase")
            ?? throw new InvalidOperationException("You must set the UvsTaskDatabase environment variable");
        private static readonly string Port = Environment.GetEnvironmentVariable("UvsTaskPort")
            ?? throw new InvalidOperationException("You must set the UvsTaskPort environment variable");
        private static readonly string SchemaLocation = Environment.GetEnvironmentVariable("UvsTaskSchemaLocation")
            ?? throw new InvalidOperationException("You must set the UvsTaskSchemaLocation environment variable");

        static async Task Main(string[] args)
        {
            Console.WriteLine("Waiting for database to start");
            await TestConnection();

            Console.WriteLine("Adding new database");
            await CreateDatabase();

            Console.WriteLine("Adding database schema");
            await ImportSchema();
        }

        private static async Task TestConnection()
        {
            Exception? latestException = null;
            var then = DateTime.UtcNow;
            while (DateTime.UtcNow - then < TimeSpan.FromMinutes(0.2))
            {
                try
                {
                    using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port};");
                    await cnxn.OpenAsync();
                    Console.WriteLine("Connection attempt succeeded");

                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connection attempt failed");
                    latestException = e;
                    await Task.Delay(1000);
                }
            }

            throw new InvalidOperationException($"Could not connect to database", latestException);
        }

        private static async Task CreateDatabase()
        {
            using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port};");
            await cnxn.OpenAsync();

            var command = cnxn.CreateCommand();
            command.CommandText = $"CREATE DATABASE {Database}";

            await command.ExecuteNonQueryAsync();
        }

        private static async Task ImportSchema()
        {
            using var cnxn = new NpgsqlConnection($"Server=localhost; User ID=postgres; Password={Password}; Port={Port}; Database={Database};");
            await cnxn.OpenAsync();

            var command = cnxn.CreateCommand();
            command.CommandText = await File.ReadAllTextAsync(SchemaLocation);

            await command.ExecuteNonQueryAsync();
        }
    }
}
