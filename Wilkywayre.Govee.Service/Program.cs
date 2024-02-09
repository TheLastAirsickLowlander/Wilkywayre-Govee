// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Serilog;
using Wilkywayre.Govee.Driver;
using Wilkywayre.Govee.Driver.Interfaces;


using var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IGoveeService, GoveeService>();
    })
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext())
    .Build();

var service = host.Services.GetService<IGoveeService>();
var logger = host.Services.GetService <ILogger<Program>>();
if (logger is null || service is null)
{
    return;
}

logger.LogDebug($"Getting devices");

var devices = await service.GetDevicesAsync();
foreach (var device in devices)
{
    logger.LogInformation($"Device: {device.MacAddress}, Turning on");
    await service.TurnOnDevice(device);
}




Microsoft.Win32.SystemEvents.SessionSwitch += (sender, e) =>
{
    if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionLock)
    {
        logger.LogInformation("Session locked");
        foreach(var device in devices)
        {
            service.TurnOffDevice(device);
        }
    }
    else if (e.Reason == Microsoft.Win32.SessionSwitchReason.SessionUnlock)
    {
        foreach(var device in devices)
        {
            service.TurnOnDevice(device);
        }
    }
};
// power off the device detection
SystemEvents.PowerModeChanged += (sender, e) =>
{
    if (e.Mode == PowerModes.Suspend)
    {
        foreach(var device in devices)
        {
            service.TurnOffDevice(device);
        }
        logger.LogInformation("Power suspend");
    }
    else if (e.Mode == PowerModes.Resume)
    {
        foreach(var device in devices)
        {
            service.TurnOnDevice(device);
        }
        logger.LogInformation("Power resume");
    }
};

SystemEvents.SessionEnding += (sender, e) =>
{
    foreach(var device in devices)
    {
        service.TurnOffDevice(device);
    }
    logger.LogInformation("Session ending: This is a logoff, shutdown, or reboot.");
};
await host.RunAsync();