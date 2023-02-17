using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Cars.Controllers;

public class CarsHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}