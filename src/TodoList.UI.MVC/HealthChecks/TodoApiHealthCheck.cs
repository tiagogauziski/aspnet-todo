using Microsoft.Extensions.Diagnostics.HealthChecks;
using TodoList.UI.MVC.TodoApiClient;

namespace TodoList.UI.MVC.HealthChecks;

public class TodoApiHealthCheck : IHealthCheck
{
    private readonly ITodoApiClient _todoApiClient;

    public TodoApiHealthCheck(ITodoApiClient todoApiClient)
    {
        _todoApiClient = todoApiClient ?? throw new ArgumentNullException(nameof(todoApiClient));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await _todoApiClient.IsHealthyAsync(cancellationToken).ConfigureAwait(false);

        return HealthCheckResult.Healthy();
    }
}
