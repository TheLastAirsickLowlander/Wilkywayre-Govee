﻿using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud.MessageHandler;

namespace Wilkywayre.Iot.Service.Services.SmartThingsCloud;

public class SmartThingsService : ISmartThingsService
{
    
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<SmartThingsService> _logger;

    public SmartThingsService(IHttpClientFactory clientFactory, ILogger<SmartThingsService> logger)
    {
        _clientFactory = clientFactory;
        _logger = logger;
    }
    
    public async ValueTask<IEnumerable<Messages.Devices.Device>> GetDevicesAsync()
    {
        var client = _clientFactory.CreateClient(nameof(SmartThingsService));
        var data = await client.GetAsync("/v1/devices");
        var content = await data.Content.ReadAsStringAsync();
        var responseObject =  JsonSerializer.Deserialize<Messages.Devices.Response>(content);
        
        _logger.LogInformation("SmartThingsService.GetDevicesAsync: {responseObject}", responseObject);
        return [];
    }
}

public static class ServiceExtensions
{
    public static IServiceCollection AddSmartThingsService(this IServiceCollection services)
    {
        services.AddOptions<Configuration.SmartThings>()
            .BindConfiguration(nameof(Configuration.SmartThings));
        services.AddTransient<SmartThingsHandler>();
        services.AddHttpClient<ISmartThingsService, SmartThingsService>(nameof(SmartThingsService), client =>
        {
            client.BaseAddress = new Uri("https://api.smartthings.com");
        }).AddHttpMessageHandler<SmartThingsHandler>();
            
        services.AddSingleton<ISmartThingsService, SmartThingsService>();
        return services;
    }    
} 