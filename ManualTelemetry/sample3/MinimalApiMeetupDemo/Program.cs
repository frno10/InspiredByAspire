using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UserManagement.Data;
using UserManagement.Models;

var builder = WebApplication.CreateBuilder(args);

// >> ADD SNIPPETS HERE (mostly)

// Add SQLite database connection
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=users.db"));

// Enable CORS (to communicate with frontend)
builder.Services.AddCors();

// Build the application
var app = builder.Build();

app.UseDefaultFiles();  // Enables "index.html" as default
app.UseStaticFiles(); // Serve index.html from wwwroot

// Enable CORS for all origins (for testing purposes)
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Create database and apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Use(async (context, next) =>
{
    await next(); // Continue processing request
});

// Routes for API Endpoints
app.MapGet("/users", async (AppDbContext db) => await db.Users.ToListAsync());

var tracer = new ActivitySource("UserManagementAPI");
app.MapPost("/users", async (AppDbContext db, User user) =>
{
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});

// Start the app
app.Run();