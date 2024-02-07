using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wilkywayre.Govee.Driver.Model;

public class GoveeWrapper
{
    public GoveeWrapper()
    {
        
    }
    public GoveeWrapper(string command, JsonElement data)
    {
        Message = new GoveeInnerRequestWrapper
        {
            Command = command,
            Data = data
        };
    }
    
    [JsonPropertyName("msg")]
    public GoveeInnerRequestWrapper Message { get; set; }
    
}
