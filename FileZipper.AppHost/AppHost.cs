var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FileZipper_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.FileZipper_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
