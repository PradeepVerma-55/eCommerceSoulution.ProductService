# eCommerce Product Service

## Overview

This solution implements a Product Service used in an eCommerce system. It is organized as a small microservice-style solution with a web API project, a data access layer and a business logic layer. The service exposes product related operations (CRUD, listing, filtering) and is meant to be consumed by other services or a front-end application.

> Target framework: .NET 8

## Repository structure

- `ProductsService.API/` — ASP.NET Core Web API project. Hosts HTTP endpoints for product operations, configuration files (e.g. `appsettings.json`) and startup wiring (DI, middleware, routing).
- `BusinessLogicLayer/` — Contains domain services, business rules and application logic used by the API. This project should be kept free of transport concerns and only depend on abstractions for persistence.
- `DataAccessLayer/` — Responsible for persistence implementation (e.g. Entity Framework Core repositories, DB context, migrations). Exposes repository interfaces consumed by `BusinessLogicLayer`.

Note: Project names and folders may vary slightly — the above reflects the projects present in this solution.

## Prerequisites

- .NET 8 SDK installed: https://dotnet.microsoft.com/en-us/download/dotnet/8.0
- Optional: database server (SQL Server, PostgreSQL, etc.) if the solution uses a relational DB. Check `appsettings.json` in `ProductsService.API` for the default connection string.

## Build and run

From the repository root, use the .NET CLI:

- Restore and build:
  - `dotnet restore`
  - `dotnet build`

- Run the API project locally:
  - `dotnet run --project ProductsService.API/ProductsService.API.csproj`

When the API runs it usually listens on a localhost port (see console output or `appsettings.Development.json`). Use tools like curl, Postman or a browser to exercise endpoints.

If you prefer to run from an IDE (Visual Studio / Rider), open the solution and run the `ProductsService.API` project.

## Configuration

- `appsettings.json` and environment-specific variants (for example `appsettings.Development.json`) contain configuration such as logging, connection strings and feature flags.
- Use environment variables to override sensitive settings in CI/CD or during local development (for example `ConnectionStrings__DefaultConnection`).

## Database and Migrations (if applicable)

If the project uses Entity Framework Core:

- Add or apply migrations from the solution root:
  - `dotnet ef migrations add <Name> --project DataAccessLayer --startup-project ProductsService.API`
  - `dotnet ef database update --project DataAccessLayer --startup-project ProductsService.API`

Adjust the project names above if your EF Core DbContext is located elsewhere.

## Logging and Observability

- The API project configures logging via the ASP.NET Core logging pipeline. Configure log levels in `appsettings.json`.
- Consider adding OpenTelemetry / tracing and metrics in production deployments.

## Docker (optional)

You can containerize the API by adding a `Dockerfile` to `ProductsService.API/` and building an image:

- Build image: `docker build -t products-service -f ProductsService.API/Dockerfile .`
- Run container: `docker run -e "ASPNETCORE_ENVIRONMENT=Production" -p 5000:80 products-service`

## Testing

- If there are unit/integration tests, run them using `dotnet test` from the repository root or the tests project folder.

## Development tips

- Keep business rules in `BusinessLogicLayer` and avoid placing business logic in controllers.
- Use interfaces for repositories in the `DataAccessLayer` and register concrete implementations via DI in `ProductsService.API`.
- Add meaningful XML or code comments for public APIs to improve maintainability.

## Contribution

- Fork the repository, create a feature branch, implement your changes and open a pull request with a clear description of the change and rationale.
- Run the build and tests locally before submitting a PR.

## License

Include a license file if the project is open source (for example `LICENSE` with an OSI-approved license). If there is an organizational or company-specific license, follow that policy.

## Contact / Further information

For questions about the code, reach out to the project owner or open an issue in this repository with details and reproduction steps.
