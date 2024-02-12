using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wilkywayre.Govee.Driver;
using Wilkywayre.Govee.Driver.Interfaces;
using Wilkywayre.Govee.Driver.Model;

namespace Wilkywayre.Iot.Service.Services.GoveeLan;

public class GoveeLanService : IControllerService
{
    private readonly IGoveeClient _goveeClient;
    private readonly ILogger<GoveeLanService> _logger;
    private readonly Configuration.GoveeLan _goveeLanOptions;
    private List<GoveeDevice> _devices { get; set; } = new();

    public GoveeLanService(IGoveeClient goveeClient, IOptions<Configuration.GoveeLan> goveeLanOptions, ILogger<GoveeLanService> logger)
    {
        _goveeClient = goveeClient;
        _logger = logger;
        _goveeLanOptions = goveeLanOptions.Value;
    }

    public async ValueTask InitializeAsync()
    {
        var devices  = await _goveeClient.GetGoveeDevicesAsync(_goveeLanOptions.ScanAttempts);
        var goveeDevices = devices as GoveeDevice[] ?? devices.ToArray();
        foreach (var device in goveeDevices)
        {
            _logger.LogInformation("Device: {DeviceMacAddress}, Turning on", device.MacAddress);            
        }
        _devices.AddRange(goveeDevices);
    }

    public async ValueTask TurnOnDevicesAsync()
    {
        List<Task<bool>> tasks = new();
        tasks.AddRange(_devices.Select(d => _goveeClient.TurnOnDeviceAsync(d)));
        await Task.WhenAll(tasks);
    }

    public async  ValueTask TurnOffDevicesAsync()
    {
        List<Task<bool>> tasks = new();
        tasks.AddRange(_devices.Select(d => _goveeClient.TurnOffDeviceAsync(d)));
        await Task.WhenAll(tasks);
    }

}
public  static partial class ServiceExtensions
{
    public static IServiceCollection AddGoveeService(this IServiceCollection services)
    {
        services.AddOptions<Configuration.GoveeLan>()
            .BindConfiguration(nameof(Configuration.GoveeLan));
        services.AddScoped<IGoveeClient, GoveeClient>();
        services.AddSingleton<IControllerService, GoveeLanService>();
        return services;
    }    
} 
