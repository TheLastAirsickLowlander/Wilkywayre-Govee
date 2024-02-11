using System.Text.Json.Serialization;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud.Messages.Devices;

public class Response
{
    [JsonPropertyName("items")]
    List<Device> Items { get; set; }
}