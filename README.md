# OpenTrivia.Api

Backend for the Open Trivia app, built with ASP.NET Core and .NET Aspire.

## Prerequisites

Install the following before running the project:

- .NET SDK 10.0
- .NET Aspire workload / tooling compatible with `Aspire.AppHost.Sdk` version `13.1.2`
- An IDE or editor with .NET support such as Visual Studio 2026 Preview or VS Code with the C# Dev Kit

Recommended checks:

```powershell
dotnet --version
dotnet workload list
```

The API projects in this repository target `net10.0`, and the Aspire AppHost project uses:

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.1.2">
```

## Repository Structure

```text
OpenTrivia.Api/
|- OpenTrivia.Api/                 # ASP.NET Core API
|- OpenTrivia.Api.AppHost/         # Aspire orchestration host
|- OpenTrivia.Api.ServiceDefaults/ # Aspire shared defaults
|- OpenTrivia.Api.slnx             # Solution file
```

If you also want to run the Angular UI, you can clone the OpenTrivia.Api repo:

```text
https://github.com/gabimousa/OpenTrivia.Frontend
```

See the readme of that reposiroty how to get up and running

## Getting Up and Running

From the root of this repository:

```powershell
cd C:\Dev\Temp\OpenTrivia\OpenTrivia.Api
dotnet restore OpenTrivia.Api.slnx
dotnet build OpenTrivia.Api.slnx
```

## Running the Project

### Option 1: Run with .NET Aspire AppHost

This is the recommended way to run the project locally.

```powershell
cd C:\Dev\Temp\OpenTrivia\OpenTrivia.Api
dotnet run --project .\OpenTrivia.Api.AppHost\OpenTrivia.Api.AppHost.csproj
```

What this does:

- Starts the Aspire AppHost
- Launches the `OpenTrivia.Api` project
- Provides the local Aspire dashboard and orchestration experience

### Option 2: Run the API directly

If you only want to run the API without Aspire orchestration:

```powershell
cd C:\Dev\Temp\OpenTrivia\OpenTrivia.Api
dotnet run --project .\OpenTrivia.Api\OpenTrivia.Api.csproj
```

In development, the API enables:

- OpenAPI document generation
- Scalar API reference UI
- CORS configured to allow local frontend development

## API Endpoints

The main endpoints are:

- `GET /api/trivia/questions`
- `POST /api/trivia/checkanswers`

Typical query parameters for `GET /api/trivia/questions`:

- `amount`
- `category`
- `difficulty`
- `type`

## Running the Frontend Together with the API

The Angular frontend can be cloned from `https://github.com/gabimousa/OpenTrivia.Frontend`.

Start the API first, then in a second terminal:

```powershell
cd [PathToDirectory]\OpenTrivia.Frontend
npm install
npm start
```

The frontend uses `proxy.conf.json` to forward `/api` calls to the local backend.

## Useful Commands

```powershell
dotnet restore OpenTrivia.Api.slnx
dotnet build OpenTrivia.Api.slnx
dotnet run --project .\OpenTrivia.Api.AppHost\OpenTrivia.Api.AppHost.csproj
dotnet run --project .\OpenTrivia.Api\OpenTrivia.Api.csproj
```

## Troubleshooting

- If `dotnet restore` fails, verify that .NET SDK 10.0 is installed.
- If the AppHost does not start, verify your Aspire installation matches the project SDK version `13.1.2`.
- If the frontend cannot reach the API, make sure the backend is running and that the frontend proxy target matches the API URL.
