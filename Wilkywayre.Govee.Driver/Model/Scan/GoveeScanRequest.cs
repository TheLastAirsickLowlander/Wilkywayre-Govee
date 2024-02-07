using System.Text.Json.Serialization;
using Wilkywayre.Govee.Driver.Interfaces;

namespace Wilkywayre.Govee.Driver.Model.Scan;

public class GoveeScanRequest : IGoveeCommand 
{
    
    
    [JsonPropertyName("account_topic")]
    public string Topic => "reserve";

    public string GetCommand() => GoveeCommands.Scan;
}
