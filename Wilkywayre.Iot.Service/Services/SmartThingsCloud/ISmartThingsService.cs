namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud;

public interface ISmartThingsService
{
    ValueTask<IEnumerable<Messages.Devices.Device>> GetDevicesAsync();
    ValueTask<bool> TurnOnDevice(Messages.Devices.Device device);
    ValueTask<bool> TurnOffDevice(Messages.Devices.Device device);

}