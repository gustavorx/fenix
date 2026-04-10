# Fenix

Fenix is a personal finance management app.

> 🚧 This project is currently under active development and subject to change.

### Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 15
- Docker Compose

### Git Workflow

This project follows a Feature Branch workflow to enable rapid iteration in a single-developer environment.

### Observability

Local observability uses:

- OpenTelemetry Collector for OTLP trace ingestion
- Prometheus for metrics scraping
- Tempo for trace storage
- Grafana for dashboards and trace exploration

Start the local infrastructure:

```powershell
docker compose up -d
```

Run the API:

```powershell
dotnet run --project src/api --launch-profile http
```

Local URLs:

- API metrics: `http://localhost:5207/metrics`
- Grafana: `http://localhost:3000`
- Prometheus: `http://localhost:9090`
- Tempo backend: `http://localhost:3200`
- Collector health: `http://localhost:13133`

Grafana local credentials come from `.env`:

- `GRAFANA_ADMIN_USER`
- `GRAFANA_ADMIN_PASSWORD`
