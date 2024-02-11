namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud;

public interface ISmartThingsService
{
    ValueTask<IEnumerable<Messages.Devices.Device>> GetDevicesAsync();

}