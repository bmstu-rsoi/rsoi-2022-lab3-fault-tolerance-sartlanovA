using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Rentals.Controllers;

public class RentalsHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
        CancellationToken cancellationToken = new CancellationToken())
    {
        return await Task.FromResult(HealthCheckResult.Healthy());
    }
}