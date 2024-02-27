#!/bin/bash

Set-Location "..\src\MyEmployeeApp.ConsoleApp"

# Set environment variables if necessary
$Env:connectionString="Server=localhost; User ID=postgres; Password=guest; Port=7777; Database=uvsproject;"

# Build the project
dotnet build

# Run commands to set and get employee data
dotnet run --no-build set-employee --employeeId 1 --employeeName John --employeeSalary 123
dotnet run --no-build set-employee --employeeId 2 --employeeName Steve --employeeSalary 456

dotnet run --no-build get-employee --employeeId 1
dotnet run --no-build get-employee --employeeId 2