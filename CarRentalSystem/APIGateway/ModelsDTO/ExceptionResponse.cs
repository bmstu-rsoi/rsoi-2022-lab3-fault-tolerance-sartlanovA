using Newtonsoft.Json;

namespace APIGateway.ModelsDTO;

public class ExceptionResponse
{
    public ExceptionResponse(string message)
    {
        Message = message;
    }
    
    [JsonProperty("Message")]
    public string Message { get; set; }
}