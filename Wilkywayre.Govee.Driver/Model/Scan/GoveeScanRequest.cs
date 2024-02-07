using System.Text.Json.Serialization;

namespace Wilkywayre.Govee.Driver.Model;

public class GoveeScanRequest 
{
    public const string Command = "scan";
    
    [JsonPropertyName("account_topic")]
    public string Topic => "reserve";
}
