# UVS-NET-TASK
Simple console application designed to manage employee data

## Overview
MyEmployeeApp is a simple console application designed to manage employee data. It allows users to add new employees to a PostgreSQL database and perform basic CRUD operations.

# Creating the Database:

Navigate to the scripts folder.
Run the PowerShell script setUpDatabase.ps1. This script will create the database in Docker.

### Applying Migrations:

Navigate to the src folder.
Navigate to MyEmployeeApp.Data.
Run the following command to apply the created migrations to Docker:

dotnet ef database update

# Functionality Testing
To verify the functionality of the project:

Navigate to the scripts folder.
Run the PowerShell script verifySubmission.ps1

### Using Command Line (Console Application):

Navigate to the MyEmployeeApp.ConsoleApp folder in the src directory.
Execute the following  commands in terminal to add and retrieve employee information:

dotnet run set-employee --employeeId 1 --employeeName John --employeeSalary 123

dotnet run --no-build set-employee --employeeId 2 --employeeName Steve --employeeSalary 456

dotnet run --no-build get-employee --employeeId 1

dotnet run --no-build get-employee --employeeId 2


# File Structure
For the current project size, the file structure might seem like overkill. However, I aimed to make it as organized and scalable as possible. By dividing the project into Core, Data, and ConsoleApp directories, and including dedicated folders for DatabaseSchema and scripts, I prioritized clarity and future scalability.


### Creator: Tomas Cerniawski
