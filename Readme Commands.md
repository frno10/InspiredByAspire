# Commands

## Containerization - Podman

```
podman run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard -e DASHBOARD__OTLP__AUTHMODE='ApiKey' -e DASHBOARD__OTLP__PRIMARYAPIKEY='meetup' mcr.microsoft.com/dotnet/aspire-dashboard:9.0

podman run --rm -it -d -p 18888:18888 -p 18890:18890 -p 4317:18889 --name aspire-dashboard -e DASHBOARD__OTLP__AUTHMODE='ApiKey' -e DASHBOARD__OTLP__PRIMARYAPIKEY='meetup' mcr.microsoft.com/dotnet/aspire-dashboard:9.0

podman run --rm -it -d -p 18888:18888 -p 18890:18890 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

## Running samples

Most of the samples require just just running `dotnet run` in a correct project
