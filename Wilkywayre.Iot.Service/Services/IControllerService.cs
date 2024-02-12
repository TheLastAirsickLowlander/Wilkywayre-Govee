namespace Wilkywayre.Iot.Service.Services;

public interface IControllerService
{
    
    ValueTask InitializeAsync();
    ValueTask TurnOnDevicesAsync();
    ValueTask TurnOffDevicesAsync();
}