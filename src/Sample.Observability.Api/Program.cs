using Prometheus;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#if DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:5260");
#endif

builder.Host.UseSerilog((context, config) =>
{
    var serviceLabel = new LokiLabel()
    {
        Key = "service",
        Value = "observability-api"
    };

    config
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.GrafanaLoki(context.Configuration.GetValue<string>("UrlGrafanaLoki")!, new List<LokiLabel>() { serviceLabel })
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
        .MinimumLevel.Override("System", LogEventLevel.Error)
        .MinimumLevel.Override("Serilog", LogEventLevel.Error)
        .MinimumLevel.Override("Prometheus", LogEventLevel.Error);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseRouting();

app.MapMetrics(); // Adiciona o endpoint /metrics do Prometheus

app.MapGet("api/v1/logs", () =>
{
    using (LogContext.PushProperty("action", "get-logs"))
    {
        Log.Logger.Information("Request received");
    }

    return Results.NoContent();
});

app.Run();
