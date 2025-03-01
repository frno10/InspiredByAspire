// using System.Diagnostics;
// using System.Diagnostics.Metrics;
// using Microsoft.EntityFrameworkCore;
// using OpenTelemetry;
// using OpenTelemetry.Logs;
// using OpenTelemetry.Metrics;
// using OpenTelemetry.Resources;
// using OpenTelemetry.Trace;
// using UserManagement.Data;
// using UserManagement.Models;

// var builder = WebApplication.CreateBuilder(args);

// // Create a custom meter (Used to define application metrics)
// var meter = new Meter("UserManagementAPI.Metrics");

// // Define some custom metrics
// var requestCounter = meter.CreateCounter<int>("users.api.requests", description: "Total number of API requests");
// var userCreationCounter = meter.CreateCounter<int>("users.created", description: "Number of users created");

// // >> ADD HERE
// string aspireEndpoint = "http://localhost:4317"; // Default for 
// builder.Logging.AddConsole();
// builder.Logging.AddOpenTelemetry(options =>
// {
//     options.IncludeFormattedMessage = true; // ✅ Include structured logs
//     options.IncludeScopes = true;
//     options.ParseStateValues = true;
//     options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UserManagementAPI"));
//     options.AddOtlpExporter(opts =>
//     {
//         opts.Endpoint = new Uri(aspireEndpoint);
//         opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//     });
// });
// // 🚀 Configure OpenTelemetry
// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(resource => resource
//         .AddService("UserManagementAPI")
//     )
//     .WithTracing(tracing => tracing
//         .AddAspNetCoreInstrumentation() // Capture HTTP requests
//         .AddEntityFrameworkCoreInstrumentation() // Capture DB Queries from EF Core
//         .AddHttpClientInstrumentation() // Track outgoing HTTP requests
//         //.AddConsoleExporter() // Logs traces to Console (debugging)
//         .AddOtlpExporter(opts =>
//         {
//             opts.Endpoint = new Uri(aspireEndpoint);
//             opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//         }) // Export to Aspire/OpenTelemetry
//     )
//     .WithMetrics(metrics => metrics
//         .AddMeter("UserManagementAPI.Metrics") // Register custom metrics
//         .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UserManagementAPI"))
//         .AddAspNetCoreInstrumentation()
//         .AddRuntimeInstrumentation()
//         //.AddConsoleExporter()
//         .AddOtlpExporter(opts =>
//         {
//             opts.Endpoint = new Uri(aspireEndpoint);
//             opts.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
//         }) // Export Metrics
//     );

// // Add SQLite database connection
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlite("Data Source=users.db"));

// // Enable CORS (to communicate with frontend)
// builder.Services.AddCors();

// // Build the application
// var app = builder.Build();

// app.UseDefaultFiles();  // Enables "index.html" as default
// app.UseStaticFiles(); // Serve index.html from wwwroot

// // Enable CORS for all origins (for testing purposes)
// app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// // Create database and apply migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

// app.Use(async (context, next) =>
// {
//     requestCounter.Add(1); // Increment request count
//     await next(); // Continue processing request
// });

// // Routes for API Endpoints
// app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());

// var tracer = new ActivitySource("UserManagementAPI");
// app.MapPost("/users", async (AppDbContext db, User user) =>
// {
//     using var activity = tracer.StartActivity("Create User");

//     activity?.AddEvent(new ActivityEvent("🔹 User API Received Request"));

//     if (string.IsNullOrWhiteSpace(user.Email))
//     {
//         activity?.AddEvent(new ActivityEvent("⚠️ Validation Failed: Email Missing"));
//         throw new Exception("Email is required");
//     }

//     activity?.AddEvent(new ActivityEvent("✅ User Data Validated"));

//     db.Users.Add(user);
//     await db.SaveChangesAsync();

//     activity?.AddEvent(new ActivityEvent("💾 User Saved to Database"));
    
//     return Results.Created($"/users/{user.Id}", user);
// });

// // Start the app
// app.Run();