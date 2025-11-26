using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

namespace FileZipper.ApiService.Features.Compression;

public static class CompressionEndpoints
{
    public static void MapCompressionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/compression");

        group.MapPost("/zip", async ([FromForm] IFormFileCollection files) =>
        {
            if (files.Count == 0)
                return Results.BadRequest("No files provided.");

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    if (file.Length == 0 || string.IsNullOrWhiteSpace(file.FileName))
                        continue;

                    var entry = archive.CreateEntry(file.FileName);
                    using var entryStream = entry.Open();
                    using var fileStream = file.OpenReadStream();
                    await fileStream.CopyToAsync(entryStream);
                }
            } 

            memoryStream.Position = 0;

            return Results.File(
                memoryStream.ToArray(), 
                "application/zip", 
                "compressed-files.zip"
            );
        })
        .DisableAntiforgery(); // simpler for API testing right now
    }
}