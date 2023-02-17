using Microsoft.Extensions.Options;
using ModelsDTO.Payments;

namespace APIGateway;

public class PaymentsSettings
{
    public Uri Host { get; set; }
}
public class PaymentsRepository : IPaymentsRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentsRepository> _logger;

    public PaymentsRepository(IOptions<PaymentsSettings> settings, HttpClient httpClient, ILogger<PaymentsRepository> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = settings.Value.Host;
        _logger = logger;
    }

    public async Task<PaymentInfo> GetAsyncByUid(Guid paymentUid)
    {
        var response = await _httpClient.GetAsync($"/api/v1/payment/{paymentUid}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentInfo>();
    }

    public async Task<PaymentInfo> CreateAsync(PaymentInfo paymentInfo)
    {
        //var response = await _httpClient.PostAsJsonAsync($"/api/v1/payment/", paymentInfo);
        var response = await _httpClient.PostAsJsonAsync($"/api/v1/payment/", paymentInfo);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentInfo>();
    }

    public async Task<PaymentInfo> CancelAsync(Guid paymentUid)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"/api/v1/payment/{paymentUid}");
        var response = await _httpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PaymentInfo>();
    }
}