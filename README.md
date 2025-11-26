# FileZipper API 🗜️

High performance file compression API (FileZipper) built with **.NET 9** and **.NET Aspire**.

## Features
* **Vertical Slice Architecture:** Modular, feature-focused.
* **Cloud-Native Orchestration:** Managed by .NET Aspire AppHost.
* **Modern Documentation:** Interactive API testing via Scalar.

## Getting Started

### Prerequisites
* .NET 9 SDK
* Docker (Optional, for future deployment)

### Running the App
1.  Clone the repository.
2.  Navigate to the root folder.
3.  Run the Aspire Orchestrator:
    ```bash
    dotnet run --project FileZipper.AppHost
    ```
4.  Open the **Aspire Dashboard** URL printed in the console.
5.  Navigate to the `apiservice` endpoint and append `/scalar/v1` to view the interactive docs.

## Endpoints

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/compression/zip` | Upload multiple files to receive a compressed `.zip` archive. |

## Tech Stack
* .NET 9 / C# 13
* .NET Aspire
* ASP.NET Core Minimal APIs
* Scalar (OpenAPI UI)