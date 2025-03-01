using Meetup.Components;
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
                .UseOtlpExporter(
                    OtlpExportProtocol.Grpc,
                    new Uri("http://localhost:4317"))
                .ConfigureResource(c => c.AddService($"RazorApp-Sample2 {DateTime.Now:hh:mm:ss}"))
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
                // .UseOtlpExporter(); same as above, which uses defaults; does not matter on order
            
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


    