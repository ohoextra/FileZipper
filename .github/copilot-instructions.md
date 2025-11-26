<!-- Copilot / AI agent instructions for contributors and automated agents -->
# FileZipper — AI Agent Instructions

Purpose: help an AI coding agent become productive quickly in this repository.

- **Big picture**: This solution uses a vertical-slice architecture orchestrated by .NET Aspire.
  - `FileZipper.AppHost` is the Aspire orchestrator that composes projects and exposes an Aspire dashboard (`AppHost.cs`).
  - `FileZipper.ApiService` is a minimal-API service exposing endpoints (see `Program.cs` and `Features/Compression/CompressionEndpoints.cs`).
  - `FileZipper.Web` is a Razor Components front-end (interactive server components) that calls the API through a named `HttpClient`.
  - `FileZipper.ServiceDefaults` contains shared conventions and telemetry/health/service-discovery wiring (`Extensions.cs`).

- **How services communicate / discovery**:
  - `AppHost` wires projects and service references; it exposes logical service names (e.g. `apiservice`) that Aspire resolves.
  - `FileZipper.Web` registers an `HttpClient` named `FileZipperApi` whose `BaseAddress` is set to `"https+http://apiservice"` in `Program.cs` — this is intentional: Aspire/service-discovery rewrites logical names at runtime.

- **Key runtime behaviors and conventions**:
  - Vertical-slice endpoints are implemented as static extension methods named `Map{Name}Endpoints(this IEndpointRouteBuilder app)` and registered from `Program.cs` (example: `MapCompressionEndpoints`).
  - Feature endpoints use minimal APIs and binder attributes (e.g., `[FromForm] IFormFileCollection files`, `[FromServices] ILogger<Program>`).
  - Upload endpoint `/api/compression/zip` (see `CompressionEndpoints.cs`) builds a ZIP in-memory and returns `Results.File(...)`. It uses `.DisableAntiforgery()` for that route.
  - Shared defaults (OpenTelemetry, health, service discovery, HTTP client resilience) are provided by calling `builder.AddServiceDefaults()` from each project's `Program.cs`.
  - Health endpoints are mapped only in Development via `MapDefaultEndpoints()` in `ServiceDefaults/Extensions.cs`.

- **Build & run (developer workflow)**:
  - Primary orchestrator (recommended):
    - `dotnet run --project FileZipper.AppHost` (runs Aspire orchestrator and composes services)
  - Run individual services for iterative work:
    - `dotnet run --project FileZipper.ApiService`
    - `dotnet run --project FileZipper.Web`
  - The Aspire dashboard printed by `AppHost` is the primary debugging/inspection UI.

- **Patterns to follow when modifying code**:
  - Add new HTTP endpoints as `MapXEndpoints` extension methods under a `Features` folder and register them from `Program.cs`.
  - Use `AddServiceDefaults()` to get consistent telemetry, health, and service-discovery behavior.
  - Prefer dependency injection via parameter injection on minimal API handlers (e.g., `[FromServices] ILogger<T>`).
  - If exposing upload endpoints, follow existing approach in `CompressionEndpoints.cs` (check for cancellation token handling and `DisableAntiforgery`).

- **Performance & safety notes (observed in code)**:
  - The compression endpoint builds the zip archive in a `MemoryStream` and returns `memoryStream.ToArray()` — be careful when adding large uploads (consider streaming or temporary file usage if sizes grow).
  - Cancellation tokens are honored and mapped to HTTP 499 when requested.

- **Where to look for examples**:
  - API route pattern: `FileZipper.ApiService/Features/Compression/CompressionEndpoints.cs` (upload + zip behavior)
  - Orchestration: `FileZipper.AppHost/AppHost.cs` (project composition and references)
  - Shared defaults: `FileZipper.ServiceDefaults/Extensions.cs` (OpenTelemetry, health checks, service discovery)
  - Front-end client example: `FileZipper.Web/Components/Pages/Zipper.razor` (uses `IHttpClientFactory`, `MultipartFormDataContent`, JS file download interop)

- **What I did *not* find**:
  - There are no unit tests or test projects in the repository. If adding tests, follow minimal-API testing patterns and mock the `HttpClient`/IFormFile abstractions.

- **Common pitfalls for agents**:
  - Don't assume `http://localhost:5000` — services are addressed by logical names under Aspire when run via `AppHost`.
  - Respect `DisableAntiforgery()` on API endpoints that accept `multipart/form-data` uploads.
  - Check `Program.cs` for `app.Environment.IsDevelopment()` guards (some endpoints such as OpenAPI UI and health checks may only be available in development).

If anything here is unclear or you want the guide to be more prescriptive (e.g., code-style rules, commit message format), tell me which areas to expand and I will iterate.
