using Xunit;
using Moq;
using MyEmployeeApp.Data;
using MyEmployeeApp.Core;
using MyEmployeeApp.ConsoleApp;
using System;

namespace MyEmployeeApp.ConsoleApp.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void HandleGetEmployeeCommand_InvalidArguments_PrintsErrorMessage()
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            var args = new string[] { "get-employee" }; // Missing employeeId argument
            var expectedErrorMessage = "Invalid or missing arguments for get-employee." + Environment.NewLine +
                                        "Usage: get-employee --employeeId <id>";

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleGetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedErrorMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleGetEmployeeCommand_InvalidEmployeeId_PrintsErrorMessage()
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            var args = new string[] { "get-employee", "--employeeId", "abc" }; // Invalid employeeId argument
            var expectedErrorMessage = "Invalid or missing arguments for get-employee." + Environment.NewLine +
                                        "Usage: get-employee --employeeId <id>";

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleGetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedErrorMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleGetEmployeeCommand_EmployeeNotFound_PrintsNotFoundMessage()
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            dbContextMock.Setup(db => db.Employees.Find(It.IsAny<int>())).Returns((Employee)null);
            var args = new string[] { "get-employee", "--employeeId", "123" };
            var expectedMessage = "Employee with ID 123 not found.";

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleGetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleGetEmployeeCommand_ValidEmployeeId_PrintsEmployeeDetails()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 123, EmployeeName = "John Doe", EmployeeSalary = 50000 };
            var dbContextMock = new Mock<IEmployeeDbContext>();
            dbContextMock.Setup(db => db.Employees.Find(123)).Returns(employee);
            var args = new string[] { "get-employee", "--employeeId", "123" };
            var expectedMessage = "Employee found: John Doe, Salary: 50000";

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleGetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedMessage, consoleOutput.GetOutput());
            }
        }

        [Theory]
        [InlineData("set-employee", "a", "--employeeName", "John", "--employeeSalary", "50000")]
        [InlineData("set-employee", "1", "--employeeName", "John", "--employeeSalary", "abc")]
        public void HandleSetEmployeeCommand_InvalidArguments_PrintsErrorMessage(string command, string id, string nameFlag, string name, string salaryFlag, string salary)
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            var args = new string[] { command, id, nameFlag, name, salaryFlag, salary };
            var expectedErrorMessage = "Invalid arguments for set-employee. EmployeeId and EmployeeSalary must be integers." + Environment.NewLine;

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleSetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedErrorMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleSetEmployeeCommand_NegativeSalary_PrintsErrorMessage()
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            var args = new string[] { "set-employee", "--employeeId", "1", "--employeeName", "John", "--employeeSalary", "-50000" };
            var expectedErrorMessage = "Employee salary cannot be negative." + Environment.NewLine;

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleSetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedErrorMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleSetEmployeeCommand_NameExceedsMaxLength_PrintsErrorMessage()
        {
            // Arrange
            var dbContextMock = new Mock<IEmployeeDbContext>();
            var args = new string[] { "set-employee", "--employeeId", "1", "--employeeName", "VeryLongEmployeeNameThatExceedsMaxLength", "--employeeSalary", "50000" };
            var expectedErrorMessage = "Employee name exceeds maximum length." + Environment.NewLine;

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleSetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedErrorMessage, consoleOutput.GetOutput());
            }
        }

        [Fact]
        public void HandleSetEmployeeCommand_EmployeeExists_PrintsDuplicateMessage()
        {
            // Arrange
            var existingEmployeeId = 1;
            var existingEmployee = new Employee { EmployeeId = existingEmployeeId, EmployeeName = "John", EmployeeSalary = 1000 };

            var dbContextMock = new Mock<IEmployeeDbContext>();
            dbContextMock.Setup(db => db.Employees.Find(existingEmployeeId)).Returns(existingEmployee);

            var args = new string[] { "set-employee", "--employeeId", "1", "--employeeName", "John", "--employeeSalary", "1000" }; // Existing employee ID
            var expectedMessage = $"Employee with ID {existingEmployeeId} already exists. Cannot add duplicate entries." + Environment.NewLine;

            // Act
            using (var consoleOutput = new ConsoleOutput())
            {
                Program.HandleSetEmployeeCommand(args, dbContextMock.Object);

                // Assert
                Assert.Contains(expectedMessage, consoleOutput.GetOutput());
            }
        }



        private class ConsoleOutput : IDisposable
        {
            private readonly System.IO.StringWriter stringWriter;
            private readonly System.IO.TextWriter originalOutput;

            public ConsoleOutput()
            {
                stringWriter = new System.IO.StringWriter();
                originalOutput = Console.Out;
                Console.SetOut(stringWriter);
            }

            public string GetOutput()
            {
                return stringWriter.ToString();
            }

            public void Dispose()
            {
                stringWriter.Dispose();
                Console.SetOut(originalOutput);
            }
        }
    }
}
