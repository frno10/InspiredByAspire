# Samples in ManualTelemetry

Samples in ManualTelemetry are connected to default standalone Aspire Dashboard running in the Podman

```
podman run --rm -it -d -p 18888:18888 -p 18890:18890 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

Samples are simply run using `dotnet run` and they are standalone apps, without AppHost project, sending data to our standalone Aspire Dashboard.

## Sample1

Sample1 is a simplest example and provides an easy insight into adding open telemetry and simply sending it via default open telemetry example

## Sample 2

Sample2 provides further insight and enables to view second application in the Aspire Dashboard, enabling us to peek into how separate resources are handled

## Sample 3

Sample 3 has a lot more into it, provide look at more complex traces with DB calls and additional service calls.
It also provides overview on introducing:

- own Counter under Meter, prividing insight in Dashboard - Metrics
- introducing own trace using ActivitySource under System.Diagnostics library

Sample3 also has a detailed Readme.md file which contains step by step guide from a plain Program.cs to get into fully developer class we want to showcase (available in Program-full.cs).
