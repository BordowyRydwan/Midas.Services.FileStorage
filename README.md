# File Storage Service
A microservice which handles file operations - upload, download, collecting info. The template contains ASP.NET + EF Core solution with test suites. By default a database and the API are set to work in Docker containers.

Following instructions are for Debian-based systems - but on Windows machines it should be similar or even simpler. 

## External ports by Docker Compose
- Web Service - 8001
- Database - 5001

## Environment used
- JetBrains Rider
- SQL Server (inside container)
- Docker

## Tech stack used
- .NET 6
- ASP.NET Core 6
- Entity Framework Core 6
- AutoMapper
- nUnit, Moq

## Getting started

To run the solution in the easiest way, do the following:

1. Install the latest [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. Install Docker Engine
  ```
  apt install docker*
  ```
3. Clone this repository
4. Run `docker-compose build && docker-compose up` inside the root folder of the solution

## Database configuration
A database is containerised by using Docker. If you have to do something outside Docker (e.g. test something on local database or with in-memory database), you'll have to adjust connection strings.

In order to do it, open the solution and edit connection string in `WebAPI/appsettings.*.json` (where `*` means config that you would like to change).

### Migrations
Default command will look like following:

```
dotnet ef migrations add "SampleMigration" --project src\Infrastructure --startup-project src\WebAPI --output-dir Infrastructure\Migrations
```

Depending on the path where you're at, the command will look different.
Migrations are applied automatically just right after running up the solution, so you will not have to update it with `Update-Database`.
