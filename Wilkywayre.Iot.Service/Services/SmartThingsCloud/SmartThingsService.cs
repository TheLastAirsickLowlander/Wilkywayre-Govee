using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud.Configuration;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud.MessageHandler;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud.Messages.Devices;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud;
/*
 * Api for the smart things api
 * https://developer.smartthings.com/docs/api/public#tag/Devices/operation/updateDevice
 */
public class SmartThingsService :  IControllerService
{
    
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<SmartThingsService> _logger;
    private readonly SmartThings _smartThingsOptions;
    private List<Messages.Devices.Device> _devices = new();
    public SmartThingsService(IHttpClientFactory clientFactory, ILogger<SmartThingsService> logger, IOptions<Configuration.SmartThings> smartThingsOptions)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _smartThingsOptions = smartThingsOptions.Value;
    }

    public async ValueTask InitializeAsync()
    {
        _logger.LogDebug($"Getting devices from SmartThingsAPI");
        var data = await GetDevicesAsync();
        var totalDevices = data as Device[] ?? data.ToArray();
        _logger.LogDebug("Retrieved {DevicesCount} devices from SmartThingsAPI", totalDevices.Count());
        _devices.AddRange(totalDevices.Where(d => _smartThingsOptions.AcceptedDevices.Contains(d.Label)));
        foreach (var device in _devices)
        {
            _logger.LogInformation("Device: {DeviceLabel}, {DeviceId}, Turning on", device.Label, device.DeviceId);            
        }
        await TurnOnDevicesAsync();

    }

    public async ValueTask TurnOnDevicesAsync()
    {
        List<Task<bool>> tasks = [];
        tasks.AddRange(_devices.Select(TurnOnDeviceAsync));
        await Task.WhenAll(tasks);
    }

    public async  ValueTask TurnOffDevicesAsync()
    {
        List<Task<bool>> tasks = new();
        tasks.AddRange(_devices.Select(TurnOffDeviceAsync));
        await Task.WhenAll(tasks);
    }
    
    private async ValueTask<IEnumerable<Device>> GetDevicesAsync()
    {
        var client = _clientFactory.CreateClient(nameof(SmartThingsService));
        var data = await client.GetAsync("/v1/devices");
        var content = await data.Content.ReadAsStringAsync();
        var responseObject =  JsonSerializer.Deserialize<ListDeviceResponse>(content);
        
        _logger.LogInformation("SmartThingsService.GetDevicesAsync: {responseObject}", responseObject);
        return responseObject.Items;
    }
    private async Task<bool> TurnOnDeviceAsync(Device device)
    {
        var client = _clientFactory.CreateClient(nameof(SmartThingsService));
        var data = await client.PostAsync($"/v1/devices/{device.DeviceId}/commands", new StringContent("{\"commands\":[{\"component\":\"main\",\"capability\":\"switch\",\"command\":\"on\"}]}"));
        var content = await data.Content.ReadAsStringAsync();
        var responseObject =  JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive  = true
        });
        
        _logger.LogInformation("SmartThingsService.TurnOnDeviceAsync: {responseObject}", responseObject);
        return true;
    }
    private async Task<bool> TurnOffDeviceAsync(Device device)
    {
        var client = _clientFactory.CreateClient(nameof(SmartThingsService));
        var data = await client.PostAsync($"/v1/devices/{device.DeviceId}/commands", new StringContent("{\"commands\":[{\"component\":\"main\",\"capability\":\"switch\",\"command\":\"off\"}]}"));
        var content = await data.Content.ReadAsStringAsync();
        var responseObject =  JsonSerializer.Deserialize<JsonElement>(content, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive  = true
        });
        
        _logger.LogInformation("SmartThingsService.TurnOnDeviceAsync: {responseObject}", responseObject);
        return true;
    }

}

public static partial class ServiceExtensions
{
    public static IServiceCollection AddSmartThingsService(this IServiceCollection services)
    {
        services.AddOptions<Configuration.SmartThings>()
            .BindConfiguration(nameof(Configuration.SmartThings));
        services.AddTransient<SmartThingsHandler>();
        services.AddHttpClient(nameof(SmartThingsService), client =>
        {
            client.BaseAddress = new Uri("https://api.smartthings.com");
        }).AddHttpMessageHandler<SmartThingsHandler>();
            
        services.AddSingleton<IControllerService, SmartThingsService>();
        return services;
    }    
} 
