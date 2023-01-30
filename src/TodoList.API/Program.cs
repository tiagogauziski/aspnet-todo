using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using TodoList.API.HealthChecks;
using TodoList.API.Infrastructure.Database;
using TodoList.API.Options;

namespace TodoList.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<TodoContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TodoListDatabase")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck>("Database");

            OpenTelemetryOptions? openTelemetryOptions = builder.Configuration.GetSection(OpenTelemetryOptions.OpenTelemetry).Get<OpenTelemetryOptions>();
            var prometheusEnabled = openTelemetryOptions?.Prometheus?.Enabled;
            builder.Services
                .AddOpenTelemetry()
                .ConfigureResource(builder => ResourceBuilder.CreateDefault().AddService(serviceName: "TodoApi", serviceNamespace: "TodoList"))
                .WithMetrics(meterProviderBuilder =>
                {
                    meterProviderBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();

                    if (prometheusEnabled.GetValueOrDefault())
                    {
                        meterProviderBuilder.AddPrometheusExporter();
                    }
                })
                .WithTracing(traceProviderBuilder =>
                {
                    traceProviderBuilder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)));

                    bool? jaegerEnabled = openTelemetryOptions?.Jaeger?.Enabled;
                    if (jaegerEnabled.GetValueOrDefault())
                    {
                        traceProviderBuilder.AddJaegerExporter();
                    }
                })
                .StartWithHost();

            var application = builder.Build();

            // Ensure database gets created
            using (var scope = application.Services.CreateScope())
            {
                var databaseContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
                databaseContext.Database.EnsureCreated();
            };

            // Configure the HTTP request pipeline.
            if (application.Environment.IsDevelopment())
            {
                application.UseSwagger();
                application.UseSwaggerUI();
            }

            application.UseHttpsRedirection();
            application.UseAuthorization();
            application.MapControllers();

            application.MapHealthChecks("/healthz/ready", new HealthCheckOptions()
            {
                Predicate = _ => true
            });
            application.MapHealthChecks("/healthz/live", new HealthCheckOptions()
            {
                Predicate = _ => false
            });
            
            if (prometheusEnabled.GetValueOrDefault())
            {
                application.UseOpenTelemetryPrometheusScrapingEndpoint();
            }

            application.Run();
        }
    }
}