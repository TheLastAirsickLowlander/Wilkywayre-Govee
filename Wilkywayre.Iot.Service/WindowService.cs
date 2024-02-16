using System.ServiceProcess;
using Microsoft.Extensions.Logging;
using Wilkywayre.Iot.Service.Services;

namespace Wilkywayre.Iot.Service;

public class WindowService : ServiceBase
{
    private readonly ILogger<WindowService> _logger;
    private readonly IEnumerable<IControllerService> _services;

    public WindowService(ILogger<WindowService> logger, IEnumerable<IControllerService> services)
    {
        _logger = logger;
        _logger.LogDebug("WindowService Constructor called");
        var arr = services.ToList();
        foreach(var service in arr)
        {
            _logger.LogInformation("Service: {ServiceName}", service.GetType().Name);
            service.InitializeAsync();
        }

        foreach (var service in arr)
        {
            _ = service.TurnOnDevicesAsync();
        }
        
        _services = arr;
        ServiceName = "Wilkywayre.Iot.Service";  
        this.CanHandleSessionChangeEvent = true;
        this.CanHandlePowerEvent = true;
        this.CanPauseAndContinue = true;
        this.CanShutdown = true;
        this.CanStop = true;
        this.AutoLog = true;
    }
    protected void OnStop()
    {
        _logger.LogInformation("Service Stopped");
    }

    protected override async void OnSessionChange(SessionChangeDescription changeDescription)
    {
        switch (changeDescription.Reason)
        {
            case SessionChangeReason.SessionLock:
                foreach (var s in _services)
                {
                    await s.TurnOffDevicesAsync();
                }
                _logger.LogInformation("Session Locked");
                break;            
            case SessionChangeReason.SessionUnlock:
                foreach (var s in _services)
                {
                    await s.TurnOnDevicesAsync();
                }
                _logger.LogInformation("Session unlocked");
                break;
            case SessionChangeReason.SessionLogon:
                foreach (var s in _services)
                {
                    await s.TurnOnDevicesAsync();
                }
                _logger.LogInformation("Session logged on");
                break;
            case SessionChangeReason.SessionLogoff:
                foreach (var s in _services)
                {
                    await s.TurnOffDevicesAsync();
                }
                _logger.LogInformation("Session logged off");
                break;
            default:
                _logger.LogInformation("Session changed");
                break;
        }
        base.OnSessionChange(changeDescription);
    }

    protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
    {
        return base.OnPowerEvent(powerStatus);
    }
}