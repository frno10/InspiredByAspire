# MinimalApi Users Meetup demo

## Project init

```
dotnet new web --name MinimalApiMeetupDemo
```

## Adding Telemetry

### Adding Aspire workload

```
dotnet workload install aspire
```

Optionally init Aspire project
```
dotnet new aspire-starter
```

### Adding required packages

```
dotnet add package Microsoft.AspNetCore.Telemetry
dotnet add package Microsoft.Extensions.Telemetry
dotnet add package Microsoft.Extensions.Http.Telemetry
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

### Adding Telemetry configuration

```
string aspireEndpoint = "http://localhost:4317"; // Default for local
// 🚀 Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("UserManagementAPI")
    )
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation() // Capture HTTP requests
        .AddEntityFrameworkCoreInstrumentation() // Capture DB Queries from EF Core
        .AddHttpClientInstrumentation() // Track outgoing HTTP requests
        .AddConsoleExporter() // Logs traces to Console (debugging)
        .AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(aspireEndpoint);
            opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }) // Export to Aspire/OpenTelemetry
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter()
        .AddOtlpExporter(opts =>
        {
            opts.Endpoint = new Uri(aspireEndpoint);
            opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        }) // Export Metrics
    );
```

## Customization

### Adding Custom Meter metric

```
// Create a custom meter (Used to define application metrics)
var meter = new Meter("UserManagement.Metrics");

// Define some custom metrics
var requestCounter = meter.CreateCounter<int>("users.api.requests", description: "Total number of API requests");
var userCreationCounter = meter.CreateCounter<int>("users.created", description: "Number of users created");

...

// in WithMetrics, add:
.AddMeter("UserManagement.Metrics") // Register custom metrics
.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UserManagementAPI"))

// also add

// Middleware to log API requests
app.Use(async (context, next) =>
{
    requestCounter.Add(1); // Increment request count
    await next(); // Continue processing request
});

// and in Post method
userCreationCounter.Add(1); // Count new user creation
```

### Add Structured Logging

```
builder.Logging.AddOpenTelemetry(options =>
{
    options.IncludeFormattedMessage = true; // ✅ Include structured logs
    options.IncludeScopes = true;
    options.ParseStateValues = true;
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UserManagementAPI"));
    options.AddOtlpExporter(opts =>
    {
        opts.Endpoint = new Uri(aspireEndpoint);
        opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
    });
});
```

### Add Console logs

Console logs only show in Aspire UI if the app is started using dotnet aspire run
Running dotnet run alone won’t capture terminal logs in Aspire UI
Structured Logs always appear via OpenTelemetry, but Process Logs require Aspire to manage the app

```
builder.Logging.AddConsole();
```

### Add Events to Trace

```
// ✅ Add Events to Traces
app.MapPost("/users", async (AppDbContext db, User user) =>
{
    using var activity = tracer.StartActivity("Create User");

    activity?.AddEvent(new ActivityEvent("🔹 User API Received Request"));

    if (string.IsNullOrWhiteSpace(user.Email))
    {
        activity?.AddEvent(new ActivityEvent("⚠️ Validation Failed: Email Missing"));
        throw new Exception("Email is required");
    }

    activity?.AddEvent(new ActivityEvent("✅ User Data Validated"));

    db.Users.Add(user);
    await db.SaveChangesAsync();

    activity?.AddEvent(new ActivityEvent("💾 User Saved to Database"));
    
    return Results.Created($"/users/{user.Id}", user);
});
```

## Modify Aspire Dashboard

### Add Action link into Aspire Dashboard to Prometheus

Update WithMetrics section to include AddPrometheusExporter

```
.WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter()  // ✅ Export to Prometheus
    );
```

Add UseOpenTelemetryPrometheusScrapingEndpoint 

```
// ✅ Expose OpenTelemetry Prometheus scraping endpoint
app.UseOpenTelemetryPrometheusScrapingEndpoint();
```

### Modify appsettings.json

```
{
  "Aspire": {
    "Dashboard": {
      "CustomActions": [
        {
          "Name": "📊 View in Prometheus",
          "UrlTemplate": "http://localhost:9090/graph?g0.expr={metric}&g0.range_input=1h",
          "ApplyTo": ["Metrics"]
        }
      ]
    }
  }
}
```

## Peek inside AddOpenTelemetry

