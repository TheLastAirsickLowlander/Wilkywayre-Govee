using System.Text.Json.Serialization;
using Wilkywayre.Govee.Driver.Interfaces;

namespace Wilkywayre.Govee.Driver.Model.Brightness;

public class GoveeBrightnessRequest : IGoveeCommand
{
    
    
    [JsonPropertyName("value")]
    public int OnOff { get; set; }

    public string GetCommand() => GoveeCommands.Brightness;
}