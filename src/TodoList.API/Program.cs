using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.Metrics;
using TodoList.API.HealthChecks;
using TodoList.API.Infrastructure.Database;

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

            builder.Services
                .AddOpenTelemetry()
                .ConfigureResource(builder => ResourceBuilder.CreateDefault().AddService(serviceName: "TodoApi"))
                .WithMetrics(builder =>
                {
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddPrometheusExporter();
                })
                .WithTracing(builder =>
                {
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSqlClientInstrumentation()
                        .AddOtlpExporter();
                })
                .StartWithHost(); ;

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

            application.UseOpenTelemetryPrometheusScrapingEndpoint();

            application.Run();
        }
    }
}