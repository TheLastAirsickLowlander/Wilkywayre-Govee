// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Serilog;
using Wilkywayre.Govee.Driver;
using Wilkywayre.Govee.Driver.Interfaces;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud;


using var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<IGoveeService, GoveeService>();
        services.AddSmartThingsService();
    })
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext())
    .Build();

var logger = host.Services.GetService <ILogger<Program>>();
if (logger is null)
{
    return;
}
logger.LogDebug($"Getting Govee Service");
var goveeService = host.Services.GetService<IGoveeService>();
logger.LogDebug("getting SmartThingsService");
var smartService =  host.Services.GetService<ISmartThingsService>();


if (logger is null || goveeService is null || smartService is null)
{
    return;
}

logger.LogDebug($"Getting devices");
var SmartDevices = await smartService.GetDevicesAsync();
var devices = await goveeService.GetDevicesAsync();
foreach (var device in devices)
{
    logger.LogInformation("Device: {DeviceMacAddress}, Turning on", device.MacAddress);
    await goveeService.TurnOnDevice(device);
}

SystemEvents.SessionSwitch += (_, e) =>
{
    switch (e.Reason)
    {
        case SessionSwitchReason.SessionLock:
        {
            logger.LogInformation("Session locked");
            foreach(var device in devices)
            {
                goveeService.TurnOffDevice(device);
            }

            break;
        }
        case SessionSwitchReason.SessionUnlock:
        {
            foreach(var device in devices)
            {
                goveeService.TurnOnDevice(device);
            }

            break;
        }
        case SessionSwitchReason.ConsoleConnect:
            break;
        case SessionSwitchReason.ConsoleDisconnect:
            break;
        case SessionSwitchReason.RemoteConnect:
            break;
        case SessionSwitchReason.RemoteDisconnect:
            break;
        case SessionSwitchReason.SessionLogon:
            break;
        case SessionSwitchReason.SessionLogoff:
            break;
        case SessionSwitchReason.SessionRemoteControl:
            break;
    }
};

// power off the device detection
SystemEvents.PowerModeChanged += (_, e) =>
{
    switch (e.Mode)
    {
        case PowerModes.Suspend:
        {
            foreach(var device in devices)
            {
                goveeService.TurnOffDevice(device);
            }
            logger.LogInformation("Power suspend");
            break;
        }
        case PowerModes.Resume:
        {
            foreach(var device in devices)
            {
                goveeService.TurnOnDevice(device);
            }
            logger.LogInformation("Power resume");
            break;
        }
        case PowerModes.StatusChange:
            break;
    }
};

SystemEvents.SessionEnding += (_, _) =>
{
    foreach(var device in devices)
    {
        goveeService.TurnOffDevice(device);
    }
    logger.LogInformation("Session ending: This is a logoff, shutdown, or reboot");
};

await host.RunAsync();