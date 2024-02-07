using System.Text.Json.Serialization;
using Wilkywayre.Govee.Driver.Interfaces;

namespace Wilkywayre.Govee.Driver.Model.Power;

public class GoveePowerRequest: IGoveeCommand
{    
    [JsonPropertyName("value")]
    public int OnOff { get; set; }

    public string GetCommand() => GoveeCommands.Power;
}