using Newtonsoft.Json;

namespace APIGateway.ModelsDTO;

public class CreateRentalRequest
{
    [JsonProperty("carUid")]
    public Guid CarUid { get; set; }
    
    [JsonProperty("dateFrom")]
    public DateTimeOffset DateFrom { get; set; }
    
    [JsonProperty("dateTo")]
    public DateTimeOffset DateTo { get; set; }
}