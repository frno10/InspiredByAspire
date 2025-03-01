# Setting up Go app with OpenTelemetry reporting to Aspire Dashboard

## Install OpenTelemetry for Go

Run the following command to install OpenTelemetry for Go:
```
go get go.opentelemetry.io/otel
go get go.opentelemetry.io/otel/exporters/otlp/otlpgrpc
go get go.opentelemetry.io/otel/trace
go get go.opentelemetry.io/otel/sdk/export/trace
go get google.golang.org/grpc
```

There appear to be some deprecated packages