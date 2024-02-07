using System.Text.Json.Serialization;

namespace Wilkywayre.Govee.Driver.Model;

public class GoveeScanResponse
{
    [JsonPropertyName("id")]
    public string Ip { get; set; }
    [JsonPropertyName("device")]
    public string Mac { get; set; }
    [JsonPropertyName("sku")]
    public string Sku { get; set; }
    [JsonPropertyName("bleVersionHard")]
    public string BluetoothVersionHard {get;set;}
    [JsonPropertyName("bleVersionSoft")]
    public string BluetoothVersionSoft {get;set;}
    [JsonPropertyName("wifiVersionHard")]
    public string WifiVersionHard {get;set;}
    [JsonPropertyName("wifiVersionSoft")]
    public string WifiVersionSoft {get;set;}
}