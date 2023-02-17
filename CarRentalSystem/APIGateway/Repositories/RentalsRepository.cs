using System.Net;
using System.Net.Http;
using System.Web;
using APIGateway.ModelsDTO;
using Microsoft.Extensions.Options;
using ModelsDTO.Rentals;

namespace APIGateway;

public class RentalsSettings
{
    public Uri Host { get; set; }
}

public class RentalsRepository : IRentalsRepository
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RentalsRepository> _logger;

    public RentalsRepository(IOptions<RentalsSettings> settings, HttpClient httpClient, ILogger<RentalsRepository> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = settings.Value.Host;
        _logger = logger;
    }
    
    public async Task<List<RentalsDTO>?> GetAllAsyncByUsername(string username)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["X-User-Name"] = username;

        var response = await _httpClient.GetAsync($"/api/v1/rental/?{query}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<RentalsDTO>>();
    }

    public async Task<RentalsDTO?> GetAsyncByUsernameAndRentalUid(string username, Guid rentalUid)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["X-User-Name"] = username;

        var response = await _httpClient.GetAsync($"/api/v1/rental/{rentalUid}/?{query}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RentalsDTO>();
    }
    
    public async Task<RentalsDTO> CreateAsync(RentalsDTO rentalDTO)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/rental/", rentalDTO);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RentalsDTO>();
    }

    public async Task<RentalsDTO> ProcessRent(string username, Guid rentalUid, string status)
    {
        var request = new HttpRequestMessage(new HttpMethod("PATCH"),
            $"/api/v1/rental/{username}/{rentalUid}/{status}");
        var response = await _httpClient.SendAsync(request);
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RentalsDTO>();
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