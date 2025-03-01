using Meetup.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class Program
{
    public static void Main(params string[]  args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

            builder.Services.AddOpenTelemetry()
                .ConfigureResource(c => c.AddService("RazorApp"))
                .WithMetrics(metrics =>
                {
                    metrics.AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                    metrics.AddAspNetCoreInstrumentation();
                })
                .WithTracing(tracing =>
                {
                    tracing.AddHttpClientInstrumentation();
                    tracing.AddAspNetCoreInstrumentation();
                });

            // Use the OTLP exporter if the endpoint is configured.
            var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            if (useOtlpExporter)
            {
                builder.Services.AddOpenTelemetry().UseOtlpExporter();
            }

        // builder.Services.AddOpenTelemetry()
        //     // .UseOtlpExporter( 
        //     //     OtlpExportProtocol.HttpProtobuf,
        //     //     new Uri("http://localhost:18888"))
        //     .WithMetrics(metrics =>
        //     {
        //         metrics.AddAspNetCoreInstrumentation(); // Use instrumentation to listen to telemetry
        //         metrics.AddHttpClientInstrumentation();
        //         metrics.AddMeter("MyMeter.Name"); // Listen to custom telemetry
        //         metrics.AddOtlpExporter(options =>
        //         {
        //             options.Endpoint = new Uri("http://localhost:18890"); // ✅ Correct Aspire Dashboard Endpoint
        //             options.Protocol = OtlpExportProtocol.HttpProtobuf;  // ✅ Use HTTP, not gRPC
        //             options.Headers = "x-otlp-api-key=meetup"; 
        //         });
        //         // metrics.AddConsoleExporter(options =>
        //         // {
        //         //     options.Targets = ConsoleExporterOutputTargets.Console; // Sends output to console
        //         // });
        //     })
        //     .WithTracing(tracing =>
        //     {
        //         tracing.AddAspNetCoreInstrumentation();
        //         tracing.AddHttpClientInstrumentation();
        //         tracing.AddOtlpExporter(options =>
        //         {
        //             options.Endpoint = new Uri("http://localhost:18890"); // ✅ Correct Aspire Dashboard Endpoint
        //             options.Protocol = OtlpExportProtocol.HttpProtobuf;  // ✅ Use HTTP, not gRPC
        //             options.Headers = "x-otlp-api-key=meetup"; 
        //         });
        //         // tracing.AddConsoleExporter(options =>
        //         // {
        //         //     options.Targets = ConsoleExporterOutputTargets.Console; // Sends output to console
        //         // });
        //     })
        //     .ConfigureResource(resource => resource.AddService("MyMeetupService"));

        // var otlpKey = new Guid("45cd644a-42ff-4f1b-9f8e-a2dfbd1133ea");
        // builder.Services.Configure<OtlpExporterOptions>(
        //     o => o.Headers = $"x-otlp-api-key=meetup");

        // Optional: Add logging to console
        builder.Services.AddLogging(logging =>
        {
            logging.AddConsole();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();


        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}


    