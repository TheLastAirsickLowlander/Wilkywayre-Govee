using System.Text.Json;
using System.Text.Json.Serialization;
using Wilkywayre.Govee.Driver.Interfaces;

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

public class GoveeWrapper<T> : GoveeWrapper where T : IGoveeCommand
{
    
    public GoveeWrapper(T data) 
    {
        Message = new GoveeInnerRequestWrapper
        {
            Command = data.GetCommand(),
            Data = JsonSerializer.SerializeToElement(data)
        };
    }
}