using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Wilkywayre.Govee.Driver.Interfaces;
using Wilkywayre.Govee.Driver.Model;
using Wilkywayre.Govee.Driver.Model.Color;
using Wilkywayre.Govee.Driver.Model.Power;
using Wilkywayre.Govee.Driver.Model.Scan;
using Wilkywayre.Govee.Driver.Model.Status;

namespace Wilkywayre.Govee.Driver;

/* Wireshark Command
 * udp && (ip.src == 192.168.1.2  ||  ip.src == 192.168.1.60) && (udp.port == 4003 || udp.port == 4001 || udp.port == 4002 )
 *
 * Details about the protocol for the govee devices
 * https://app-h5.govee.com/user-manual/wlan-guide
 */

public class GoveeService : IGoveeService
{
    private readonly ILogger<GoveeService> _logger;
    private readonly CancellationTokenSource _tokenSource;
    private List<GoveeDevice> Devices { get; set; } = new();

    public GoveeService(ILogger<GoveeService> logger)
    {
        _logger = logger;
        _tokenSource = new CancellationTokenSource();
        var init = Initialize();
        init.Wait();

    }


    private async Task Initialize()
    {
        await GetGoveeDevices();
    }

    private async Task GetGoveeDevices()
    {
        var receiveTask = ListForDevices(_tokenSource.Token);
        foreach (var i in Enumerable.Range(0, 2))
        {
            SendRequestScan();
            await Task.Delay(1000);   
        }
    }
    private async Task ListForDevices(CancellationToken token = default)
    {
        GoveeScanResponse? message = null;
        try
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 4002);
            using (var udpClient = new UdpClient())
            {
                udpClient.Client.Bind(ipEndPoint);
                do
                {
                    var data = await udpClient.ReceiveAsync(token);
                    try
                    {
                        var wrapper = JsonSerializer.Deserialize<GoveeWrapper>(data.Buffer);
                        if (wrapper is null ||wrapper.Message.Command != GoveeCommands.Scan)
                        {
                            continue;
                        }

                        message = JsonSerializer.Deserialize<GoveeScanResponse>(wrapper.Message.Data);

                        if (message is null)
                        {
                            continue;
                        }                        
                        _logger.LogDebug("Received message from {IpAddress} with {Mac}", message.Ip, message.Mac);
                        if (Devices.All(d => d.MacAddress != message.Mac))
                        {
                            var gd = new GoveeDevice
                            {
                                IPAddress = message.Ip,
                                MacAddress = message.Mac,
                            };
                            Devices.Add(gd);    
                        }
                    }
                    catch (JsonException err)
                    {
                        _logger.LogError(err, "Error parsing message");
                    }
                } while (!token.IsCancellationRequested);
            }
        }
        catch(Exception err)
        {
            _logger.LogError(err, "Unexpected Error happened" );
        }
    }

    private void SendRequestScan()
    {
        using( var udpClient = new UdpClient(4001))
        {
            var endPoint = new IPEndPoint(IPAddress.Broadcast, 4001);

            var message = new GoveeWrapper<GoveeScanRequest>(new());

            var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
            udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
            _logger.LogDebug("Sending scan request");
        }
    }

    private async Task SendPowerOnRequest(GoveeDevice device, int onOff)
    {
        using var udpClient = new UdpClient(4003);
        var endPoint = new IPEndPoint(IPAddress.Parse(device.IPAddress), 4003);
        // turns it on
        _logger.LogDebug("Sending power on request to {DeviceIpAddress} with {OnOff}", device.IPAddress, onOff);
        var message = new GoveeWrapper<GoveePowerRequest>(new() { OnOff = onOff });
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        await udpClient.SendAsync(requestScanMessage, requestScanMessage.Length, endPoint);
    }

    private void SendRequestStatus(GoveeDevice device)
    {
        using var udpClient = new UdpClient(4003);
        var endPoint = new IPEndPoint(IPAddress.Parse(device.IPAddress), 4003);

        var message = new GoveeWrapper<GoveeStatusRequest>(new());
        _logger.LogDebug("Sending status request to {DeviceIpAddress}", device.IPAddress);
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
    }


    public Task<List<GoveeDevice>> GetDevicesAsync()
    {
        return Task.FromResult(Devices);
    }

    public async Task<bool> TurnOnDevice(GoveeDevice device)
    {
        await SendPowerOnRequest(device, 1);
        return true;
    }

    public async Task<bool> TurnOffDevice(GoveeDevice device)
    {
        await SendPowerOnRequest(device, 0);
        return true;
    }

    public Task<bool> SetColor(GoveeDevice device, GoveeColor color)
    {
        throw new NotImplementedException();
    }
}