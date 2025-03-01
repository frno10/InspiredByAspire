# Inspired by Aspire

Repo for Meetup on topic Inspired by Aspire, 27.2.2025

Presentation is [here|https://github.com/frno10/InspiredByAspire/blob/main/Inspired%20by%20Aspire%20NEW.pdf]
Older presentation focused on OpenTelemetry is also included [here|https://github.com/frno10/InspiredByAspire/blob/main/Code%20%26%20Coffee%20-%20OpenTelemetry.pdf]

## Sample in AspireApplication

Default sample running `dotnet new aspire-starter --name MeetupDemo`

Showcasing default and super easy & time efficient way to make the basic demo work.
Showcases the ability to have such app up & running in couple minutes.

## eShop

Microsoft eShop on containers continues to be great source of knowledge and example, and provides a fantastic look on the capability of Aspire.

## Samples in ManualTelemetry

Samples in ManualTelemetry are connected to default standalone Aspire Dashboard running in the Podman

```
podman run --rm -it -d -p 18888:18888 -p 18890:18890 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

Samples are simply run using `dotnet run` and they are standalone apps, without AppHost project, sending data to our standalone Aspire Dashboard.

### Sample1

Sample1 is a simplest example and provides an easy insight into adding open telemetry and simply sending it via default open telemetry example

### Sample 2

Sample2 provides further insight and enables to view second application in the Aspire Dashboard, enabling us to peek into how separate resources are handled

### Sample 3

Sample 3 has a lot more into it, provide look at more complex traces with DB calls and additional service calls.
It also provides overview on introducing:

- own Counter under Meter, prividing insight in Dashboard - Metrics
- introducing own trace using ActivitySource under System.Diagnostics library

Sample3 also has a detailed Readme.md file which contains step by step guide from a plain Program.cs to get into fully developer class we want to showcase (available in Program-full.cs).
