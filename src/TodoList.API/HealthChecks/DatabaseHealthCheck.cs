using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoList.API.Infrastructure.Database;

namespace TodoList.API.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly TodoContext _todoContext;

        public DatabaseHealthCheck(TodoContext todoContext)
        {
            _todoContext = todoContext ?? throw new ArgumentNullException(nameof(todoContext));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var canConnectDatabase = await _todoContext.Database.CanConnectAsync(cancellationToken);

            if (canConnectDatabase)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy("Unable to connect to database.");
        }
    }
}
