FROM prom/prometheus AS prometheus

COPY prometheus.yml /etc/prometheus/prometheus.yml

FROM grafana/loki:latest AS loki

COPY loki-config.yml /etc/loki/loki-config.yml