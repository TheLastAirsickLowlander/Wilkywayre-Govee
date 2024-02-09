// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Wilkywayre.Govee.Driver;
using Wilkywayre.Govee.Driver.Interfaces;


using IHost host = Host.CreateDefaultBuilder(args)
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
    logger.LogInformation($"Device: {device.MacAddress}");
}
