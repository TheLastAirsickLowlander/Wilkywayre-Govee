using Wilkywayre.Govee.Driver.Model;
using Wilkywayre.Govee.Driver.Model.Color;

namespace Wilkywayre.Govee.Driver.Interfaces;

public interface IGoveeClient
{
    Task<IEnumerable<GoveeDevice>> GetGoveeDevicesAsync(int requestAttempts = 2);
    public Task<bool> TurnOnDeviceAsync(GoveeDevice device);
    public Task<bool> TurnOffDeviceAsync(GoveeDevice device);
    public Task<bool> SetColorAsync(GoveeDevice device, GoveeColor color);
    
}