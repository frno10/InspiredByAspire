# Commands

## Add Prometheus to Podman

```
podman run --name prometheus -p 9090:9090 --rm -d -v $(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml docker.io/prom/prometheus
```