using Wilkywayre.Govee.Driver.Model;
using Wilkywayre.Govee.Driver.Model.Color;

namespace Wilkywayre.Govee.Driver.Interfaces;

public interface IGoveeService
{
    public Task<List<GoveeDevice>> GetDevicesAsync();
    public Task<bool> TurnOnDevice(GoveeDevice device);
    public Task<bool> TurnOffDevice(GoveeDevice device);
    public Task<bool> SetColor(GoveeDevice device, GoveeColor color);
    
}