# FileZipper

High-performance file compression service built with **.NET 10**, **Blazor Server**, and **.NET Aspire** orchestration.

## Features

- **Vertical Slice Architecture:** Modular, feature-focused endpoint organization.
- **Cloud-Native Orchestration:** Multi-project composition via .NET Aspire AppHost.
- **Blazor Server UI:** Interactive file selection and download directly from the browser.
- **Interactive API Docs:** Scalar OpenAPI UI in development mode.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Windows PowerShell 5.1 or PowerShell Core (for terminal commands)
- Dev HTTPS certificate (auto-trusted by dotnet dev-certs https --trust)

### Running the Application

#### Option 1: Using Aspire Orchestrator (Recommended)

`powershell
dotnet run --project FileZipper.AppHost
`

This starts both the Web UI and API service through Aspire service discovery. Open the Aspire Dashboard URL printed to the console (typically https://localhost:17087/login).

#### Option 2: Run Services Individually (Development)

In separate terminals:

`powershell
# Terminal 1: Start ApiService
dotnet run --project FileZipper.ApiService
`

`powershell
# Terminal 2: Start Web UI
dotnet run --project FileZipper.Web
`

The Web UI will be available at https://localhost:7009. In development mode, the client automatically posts to https://localhost:7484 for the API.

### Building the Solution

`powershell
dotnet build FileZipper.sln
`

## API Endpoints

| Method | Endpoint | Description | Content-Type |
| :--- | :--- | :--- | :--- |
| POST | /api/compression/zip | Upload multiple files to receive a compressed .zip archive. | multipart/form-data |

**Request:** Multipart form with iles field containing one or more files.

**Response:** Binary .zip file with Content-Disposition: attachment header.

## Project Structure

`
FileZipper/
 FileZipper.AppHost/           # Aspire orchestrator
 FileZipper.ApiService/        # Minimal API (compression endpoint)
    Features/Compression/     # Vertical-slice endpoint
 FileZipper.Web/               # Blazor Server interactive components
    Components/Pages/Zipper.razor
    wwwroot/app.js            # Client-side upload helper
 FileZipper.ServiceDefaults/   # Shared telemetry & service discovery
 .github/copilot-instructions.md
`

## Development Notes

### Service Discovery & Ports

- **ApiService:**
  - HTTP: http://localhost:5499
  - HTTPS: https://localhost:7484
- **Web UI:**
  - HTTPS: https://localhost:7009

In production (via Aspire), services use logical names (https+http://apiservice) and Aspire resolves them automatically.

### File Size Limits

- **Client-side limit:** 1000 MB (defined in Zipper.razor)
- **Server-side limit:** Configured by the running process (SignalR buffer size for streaming)

## Tech Stack

- **.NET 10** with C# 14
- **Blazor Server** (interactive server-side components)
- **.NET Aspire** (multi-project orchestration)
- **ASP.NET Core Minimal APIs**
- **OpenTelemetry** (distributed tracing & metrics)
- **Scalar** (interactive OpenAPI documentation)
- **System.IO.Compression** (native ZIP support)

