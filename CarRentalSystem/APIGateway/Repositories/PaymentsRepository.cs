using System.Net;
using MassTransit;
using Microsoft.Extensions.Options;
using ModelsDTO.Payments;
using ModelsDTO.Payments.Cancel;

namespace APIGateway;

public class PaymentsSettings
{
    public Uri Host { get; set; }
}
public class PaymentsRepository : IPaymentsRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentsRepository> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PaymentsRepository(IOptions<PaymentsSettings> settings, HttpClient httpClient, ILogger<PaymentsRepository> logger,
        IServiceProvider serviceProvider)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = settings.Value.Host;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task<PaymentInfo> GetAsyncByUid(Guid paymentUid)
    {
        var response = await _httpClient.GetAsync($"/api/v1/payment/{paymentUid}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentInfo>();
    }

    public async Task<PaymentInfo> CreateAsync(PaymentInfo paymentInfo)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/payment/", paymentInfo);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentInfo>();
    }

    public async Task CancelAsync(Guid paymentUid)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
        
        await publisher.Publish<CancelPayment>(new {PaymentUid = paymentUid});

        // try
        // {
        //     var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/api/v1/payment/{paymentUid}");
        //     var response = await _httpClient.SendAsync(request);
        //
        //     response.EnsureSuccessStatusCode();
        //     return await response.Content.ReadFromJsonAsync<PaymentInfo>();
        // }
        // catch (Exception e)
        // {
        //     return null;
        // }
    }
    
    public async Task<bool> HealthCheckAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"manage/health");
            var response = await _httpClient.SendAsync(request);
            return (response.StatusCode == HttpStatusCode.OK);
        }
        catch (Exception e)
        {
            return false;
        }
    }
}