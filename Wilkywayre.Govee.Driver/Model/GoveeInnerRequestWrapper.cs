using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wilkywayre.Govee.Driver.Model;

public class GoveeInnerRequestWrapper
{
    [JsonPropertyName("cmd")]
    public string Command { get; set; }
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }
}