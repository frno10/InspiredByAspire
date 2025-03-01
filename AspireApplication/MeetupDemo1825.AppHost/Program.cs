var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder
.AddProject<Projects.MeetupDemo1825_ApiService>("apiservice");

builder.AddProject<Projects.MeetupDemo1825_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
