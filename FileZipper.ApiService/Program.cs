using FileZipper.ApiService.Features.Compression;
using Scalar.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add a permissive development CORS policy so the Web UI (different origin/port)
// can POST multipart/form-data to the API during development.
const string DevCorsPolicyName = "DevCors";
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(DevCorsPolicyName, policy =>
        {
            policy.WithOrigins("https://localhost:7009", "http://localhost:7009")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });
}

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // Enable the development CORS policy for local testing.
    app.UseCors(DevCorsPolicyName);

    // Enable OpenAPI & API reference only in development
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();

app.MapCompressionEndpoints();

app.Run();
