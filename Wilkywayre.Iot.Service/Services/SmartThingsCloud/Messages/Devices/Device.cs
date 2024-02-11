using System.Text.Json.Serialization;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud.Messages.Devices;

public class Device
{
    [JsonPropertyName("deviceId")]
    public string DeviceId { get; set; }

    // [JsonPropertyName("name")]
    // public string Name { get; set; }
    //
    // [JsonPropertyName("label")]
    // public string Label { get; set; }
    //
    // [JsonPropertyName("manufacturerName")]
    // public string ManufacturerName { get; set; }
    //
    // [JsonPropertyName("presentationId")]
    // public string PresentationId { get; set; }
    //
    // [JsonPropertyName("deviceManufacturerCode")]
    // public string DeviceManufacturerCode { get; set; }
    //
    // [JsonPropertyName("locationId")]
    // public string LocationId { get; set; }
    //
    // [JsonPropertyName("ownerId")]
    // public string OwnerId { get; set; }
    //
    // [JsonPropertyName("roomId")]
    // public string RoomId { get; set; }

}

