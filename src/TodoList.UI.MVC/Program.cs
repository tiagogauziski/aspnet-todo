using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TodoList.UI.MVC.Extentions;
using TodoList.UI.MVC.HealthChecks;
using TodoList.UI.MVC.Options;
using TodoList.UI.MVC.TodoApiClient;

namespace TodoList.UI.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.Configure<TodoApiOptions>(builder.Configuration.GetSection(TodoApiOptions.TodoApi));
            builder.Services.Configure<TodoApplicationOptions>(builder.Configuration.GetSection(TodoApplicationOptions.TodoApplication));
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
            builder.Services.AddHttpClient<ITodoApiClient, TodoApiClient.TodoApiClient>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddHealthChecks()
                .AddCheck<TodoApiHealthCheck>("TodoUiMvc");

            OpenTelemetryOptions? openTelemetryOptions = builder.Configuration.GetSection(OpenTelemetryOptions.OpenTelemetry).Get<OpenTelemetryOptions>();
            var prometheusEnabled = openTelemetryOptions?.Prometheus?.Enabled;
            builder.Services
                .AddOpenTelemetry()
                .ConfigureResource(builder => ResourceBuilder.CreateDefault().AddService(serviceName: "TodoApi"))
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

            var applicationOptions = application.Services.GetRequiredService<IOptions<TodoApplicationOptions>>();
            if (applicationOptions is not null)
            {
                application.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(applicationOptions.Value.BasePath);
                    return next(context);
                });
            }

            application.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            if (!application.Environment.IsDevelopment())
            {
                application.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                application.UseHsts();
            }

            application.UseHttpsRedirection();
            application.UseStaticFiles();

            application.UseRouting();

            application.UseAuthorization();

            application.MapDebugPage("/Debug");

            application.MapControllerRoute(
                name: "default",
                pattern: "{controller=Todo}/{action=Index}/{id?}");

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