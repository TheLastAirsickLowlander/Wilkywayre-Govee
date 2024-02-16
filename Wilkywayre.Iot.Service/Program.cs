// See https://aka.ms/new-console-template for more information

using System.ServiceProcess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Serilog;
using Wilkywayre.Iot.Service;
using Wilkywayre.Iot.Service.Services;
using Wilkywayre.Iot.Service.Services.GoveeLan;
using Wilkywayre.Iot.Service.Services.SmartThingsCloud;



using var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureHostConfiguration(configHost =>
    {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", optional: true);
    })
    .ConfigureServices((_, services) =>
    {
        services.AddSingleton<WindowService>();
        services.AddGoveeService();
        services.AddSmartThingsService();
    })
    .UseSerilog((context, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext())
    .Build();

// logger.LogDebug($"Getting IController Services");
// var services = host.Services.GetServices<IControllerService>();
//
// var controllerServices = services as IControllerService[] ?? services.ToArray();
// if(!controllerServices.Any())
// {
//     logger.LogError("No controllers found");
//     return;
// }
//
// foreach (var service in controllerServices)    
// {    
//     logger.LogInformation("Initializing {ServiceName}", service.GetType().Name);
//     await service.InitializeAsync();
// }
//
// SystemEvents.SessionSwitch += (_, e) =>
// {
//     switch (e.Reason)
//     {
//         case SessionSwitchReason.SessionLock:
//         {
//             logger.LogInformation("Session locked");
//             foreach (var service in controllerServices)
//             {
//                 service.TurnOffDevicesAsync();
//             }
//             
//             break;
//         }
//         case SessionSwitchReason.SessionUnlock:
//         {
//             logger.LogInformation("Session unlocked");
//             foreach (var service in controllerServices)
//             {
//                 service.TurnOnDevicesAsync();
//             }
//
//             break;
//         }
//         case SessionSwitchReason.ConsoleConnect:
//             break;
//         case SessionSwitchReason.ConsoleDisconnect:
//             break;
//         case SessionSwitchReason.RemoteConnect:
//             break;
//         case SessionSwitchReason.RemoteDisconnect:
//             break;
//         case SessionSwitchReason.SessionLogon:
//             break;
//         case SessionSwitchReason.SessionLogoff:
//             break;
//         case SessionSwitchReason.SessionRemoteControl:
//             break;
//     }
// };
//
// // power off the device detection
// SystemEvents.PowerModeChanged += (_, e) =>
// {
//     switch (e.Mode)
//     {
//         case PowerModes.Suspend:
//         {
//             logger.LogInformation("Power suspend");
//             foreach (var service in controllerServices)
//             {
//                 service.TurnOffDevicesAsync();
//             }
//
//             break;
//         }
//         case PowerModes.Resume:
//         {
//             logger.LogInformation("Power resume");
//             foreach (var service in controllerServices)
//             {
//                 service.TurnOnDevicesAsync();
//             }
//
//             break;
//         }
//         case PowerModes.StatusChange:
//             break;
//     }
// };
//
// SystemEvents.SessionEnding += (_, _) =>
// {
//     logger.LogInformation("Session ending: This is a logoff, shutdown, or reboot");
//     foreach (var service in controllerServices)
//     {
//         service.TurnOffDevicesAsync();
//     }
// };
var ws = host.Services.GetService<WindowService>();
ServiceBase.Run(ws);