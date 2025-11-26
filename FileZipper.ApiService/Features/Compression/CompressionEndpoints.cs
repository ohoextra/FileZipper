using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace FileZipper.ApiService.Features.Compression;

public static class CompressionEndpoints
{
    public static void MapCompressionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/compression");

        // DEFINITION: The "Zip" Endpoint
        group.MapPost("/zip", async ([FromForm] IFormFileCollection files) =>
        {
            if (files.Count == 0)
                return Results.BadRequest("No files provided.");

            // 1. Create a memory stream to hold the zip file in RAM
            using var memoryStream = new MemoryStream();

            // 2. Create the archive
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    // Add each uploaded file to the zip
                    var entry = archive.CreateEntry(file.FileName);
                    
                    using var entryStream = entry.Open();
                    using var fileStream = file.OpenReadStream();
                    await fileStream.CopyToAsync(entryStream);
                }
            } // Archive is explicitly closed/disposed here, finalizing the zip data

            // 3. Reset stream position to the beginning so we can read it
            memoryStream.Position = 0;

            // 4. Return the file
            return Results.File(
                memoryStream.ToArray(), 
                "application/zip", 
                "compressed-files.zip"
            );
        })
        .DisableAntiforgery(); // simpler for API testing right now
    }
}