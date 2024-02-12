using System.Text.Json.Serialization;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud.Messages.Devices;

public class ListDeviceResponse
{
    [JsonPropertyName("items")]
    public Device[] Items { get; set; }
}