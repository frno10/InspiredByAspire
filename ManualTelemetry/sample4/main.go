package main

import (
	"context"
	"log"
	"time"

	"go.opentelemetry.io/otel"
	"go.opentelemetry.io/otel/exporters/otlp/otlptrace"
	"go.opentelemetry.io/otel/exporters/otlp/otlpmetric"
	"go.opentelemetry.io/otel/exporters/otlp/otlpgrpc"
	"go.opentelemetry.io/otel/sdk/resource"
	"go.opentelemetry.io/otel/sdk/trace"
	"go.opentelemetry.io/otel/sdk/metric"
	"google.golang.org/grpc"
)

// ✅ Configure OpenTelemetry Tracing
func initTracer() (*trace.TracerProvider, error) {
	endpoint := "localhost:4317" // 🚀 Aspire OTLP gRPC endpoint

	// ✅ Set up OTLP Tracing Exporter
	traceExporter, err := otlptrace.New(context.Background(),
		otlpgrpc.NewClient(
			otlpgrpc.WithEndpoint(endpoint),
			otlpgrpc.WithInsecure(), // No TLS required for Aspire
		),
	)
	if err != nil {
		return nil, err
	}

	// ✅ Set up Tracer Provider
	tp := trace.NewTracerProvider(
		trace.WithBatcher(traceExporter),
		trace.WithResource(resource.Default()),
	)
	otel.SetTracerProvider(tp)
	return tp, nil
}

// ✅ Configure OpenTelemetry Metrics
func initMetrics() (*metric.MeterProvider, error) {
	endpoint := "localhost:4317"

	// ✅ Set up OTLP Metric Exporter
	metricExporter, err := otlpmetric.New(context.Background(),
		otlpgrpc.NewClient(
			otlpgrpc.WithEndpoint(endpoint),
			otlpgrpc.WithInsecure(),
		),
	)
	if err != nil {
		return nil, err
	}

	// ✅ Set up Meter Provider
	mp := metric.NewMeterProvider(
		metric.WithReader(metric.NewPeriodicReader(metricExporter)),
	)
	otel.SetMeterProvider(mp)
	return mp, nil
}

// ✅ Sends a Sample Trace
func runSampleTrace() {
	tracer := otel.Tracer("Go-App")
	ctx, span := tracer.Start(context.Background(), "TestTrace")
	defer span.End()

	time.Sleep(500 * time.Millisecond)
	log.Println("📡 Sent trace from Go to Aspire Dashboard")
}

func main() {
	// ✅ Initialize OpenTelemetry Tracing
	tp, err := initTracer()
	if err != nil {
		log.Fatalf("Failed to initialize tracer: %v", err)
	}
	defer tp.Shutdown(context.Background())

	// ✅ Initialize OpenTelemetry Metrics
	mp, err := initMetrics()
	if err != nil {
		log.Fatalf("Failed to initialize metrics: %v", err)
	}
	defer mp.Shutdown(context.Background())

	// ✅ Run sample trace
	runSampleTrace()

	log.Println("🚀 Go App completed")
}