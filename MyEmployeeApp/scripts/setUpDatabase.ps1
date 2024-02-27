# Set environment variables
$Env:UvsTaskPassword = "guest"
$Env:UvsTaskDatabase = "uvsproject"
$Env:UvsTaskPort = "7777"
$Env:UvsTaskSchemaLocation = "$(Get-Location)\..\DatabaseSchema\dbSchema.sql"

# Display message
Write-Host ""
Write-Host "This script will set up a PostgreSQL database hosted in a Docker container."
Read-Host "Press ENTER to continue"

# Pull the PostgreSQL Docker image
docker pull postgres
if (-not $?) { exit 1 }

# Define port assignment
$portAssign = $Env:UvsTaskPort + ":5432"

# Start a PostgreSQL container
$container = $(docker run -e "POSTGRES_PASSWORD=$Env:UvsTaskPassword" -p "$portAssign" -d postgres)
if (-not $?) { exit 1 }

Try {
    # Inform about the database starting and schema setting
    Write-Host "Database starting. Setting database schema..."

    # Get the base directory of the script
    $baseDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path

    # Move to the DatabaseSchema directory
    Push-Location "$baseDirectory\..\DatabaseSchema"

    # Run the program to set the schema
    dotnet run
    if (-not $?) { exit 1 }

    # Display connection string and schema applied
    Write-Host ""
    Write-Host "The database is ready to use." -ForegroundColor Green
    Write-Host "Connection string: 'Server=localhost; User ID=postgres; Password=$Env:UvsTaskPassword; Port=$Env:UvsTaskPort; Database=$Env:UvsTaskDatabase;'"
    Write-Host "Schema applied to database:"
    Get-Content ./dbSchema.sql
    Write-Host ""
    Write-Host "Press Ctrl^C to stop the database server and exit." -ForegroundColor Green

    # Attach to the Docker container
    docker attach "$container"
} Finally {
    # Return to the previous location and stop the container
    Pop-Location
    docker stop "$container"
}
