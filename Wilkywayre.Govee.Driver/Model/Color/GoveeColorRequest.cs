using System.Text.Json.Serialization;
using Wilkywayre.Govee.Driver.Interfaces;

namespace Wilkywayre.Govee.Driver.Model.Color;

public class GoveeColorRequest : IGoveeCommand
{
    
    
    [JsonPropertyName("Color")]
    public GoveeColor Color { get; set; }
    
    [JsonPropertyName("colorTemInKelvin")]
    public int ColorTemperatureInKelvin { get; set; }
    
    public string GetCommand() => GoveeCommands.Color;
}
