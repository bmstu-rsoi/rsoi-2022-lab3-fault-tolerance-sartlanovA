using ModelsDTO.Payments;
using Newtonsoft.Json;

namespace APIGateway.ModelsDTO;

public class CreateRentalResponse
{
    [JsonProperty("rentalUid")]
    public Guid RentalUid { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
    
    [JsonProperty("carUid")]
    public Guid CarUid { get; set; }
    
    [JsonProperty("dateFrom")]
    public string DateFrom { get; set; }
    
    [JsonProperty("dateTo")]
    public string DateTo { get; set; }
    
    [JsonProperty("payment")]
    public PaymentInfo Payment { get; set; }
}