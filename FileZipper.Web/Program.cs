using FileZipper.Web;
using FileZipper.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Server-Side Blazor hub options. We register both Razor Components (interactive server)
// and ServerSideBlazor hub options because the app uses SignalR streaming for some interactions.
// These settings increase timeouts and message sizes to reduce the likelihood of streaming timeouts
// during large uploads (still prefer direct browser uploads when possible).
builder.Services.AddServerSideBlazor().AddHubOptions(options =>
{
    // Allow longer client timeouts during large uploads
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(2);
    // Increase maximum message size (bytes) for streamed file chunks
    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 MB per message
});

if (builder.Environment.IsDevelopment())
{
    // ApiService exposes HTTP on port 5499 in the default launch profile; use plain HTTP here
    builder.Services.AddHttpClient("FileZipperApi", client =>
        client.BaseAddress = new Uri("http://localhost:5499"));
}
else
{
    builder.Services.AddHttpClient("FileZipperApi", client =>
        client.BaseAddress = new Uri("https+http://apiservice"));
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
