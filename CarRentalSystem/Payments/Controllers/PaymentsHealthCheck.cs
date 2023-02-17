using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Payments.Controllers;

public class PaymentsHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}