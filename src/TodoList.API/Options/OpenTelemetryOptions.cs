namespace TodoList.API.Options
{
    public class OpenTelemetryOptions
    {
        public const string OpenTelemetry = "OpenTelemetry";

        public JaegerOptions? Jaeger { get; set; }

        public PrometheusOptions? Prometheus { get; set; }
    }
}
