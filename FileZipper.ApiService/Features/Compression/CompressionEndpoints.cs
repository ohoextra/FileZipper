using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace FileZipper.ApiService.Features.Compression;

public static class CompressionEndpoints
{
    public static void MapCompressionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/compression");

        group.MapPost("/zip", async (
            [FromForm] IFormFileCollection files,
            [FromServices] ILogger<Program> logger, 
            CancellationToken ct) =>
        {
            if (files is null || !files.Any())
            {
                logger.LogInformation("No files provided by the user.");
                return Results.BadRequest("No files provided.");
            }
            try
            {
                logger.LogInformation("Starting file compression..");

                // Build the ZIP archive in a MemoryStream and return the stream directly.
                // Avoid calling ToArray() to prevent a second in-memory copy of the archive.
                var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            logger.LogInformation("Zipping operation was cancelled by the user.");
                            return Results.StatusCode(499); // "Client Closed Request"
                        }

                        if (string.IsNullOrWhiteSpace(file.FileName))
                            continue;

                        var entry = archive.CreateEntry(file.FileName);
                        using var entryStream = entry.Open();
                        using var fileStream = file.OpenReadStream();
                        await fileStream.CopyToAsync(entryStream, ct);
                    }
                }
                memoryStream.Position = 0;

                logger.LogInformation("File compression completed successfully.");
                return Results.File(
                    memoryStream,
                    "application/zip",
                    "compressed-files_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss") + ".zip"
                );
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Zipping operation was cancelled by the user.");
                return Results.StatusCode(499); // 499 = "Client Closed Request"
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while zipping files.");
                return Results.Problem("An unexpected error occurred while processing your files.");
            }
        })
        .DisableAntiforgery();
    }
}