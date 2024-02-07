using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Wilkywayre.Govee.Driver.Model;
using Wilkywayre.Govee.Driver.Model.Power;
using Wilkywayre.Govee.Driver.Model.Scan;
using Wilkywayre.Govee.Driver.Model.Status;

namespace Wilkywayre.Govee.Driver;

public class GoveeClient
{
    /* Wireshark Command
     * _ws.col.protocol == "UDP" && (ip.src == 192.168.1.2  ||  ip.src == 192.168.1.60) && (udp.port == 4003 || udp.port == 4001 || udp.port == 4002 )
     *
     * Details about the protocol for the govee devices
     * https://app-h5.govee.com/user-manual/wlan-guide
     */
    
    
    public GoveeClient()
    {
        var client = ListenForClients(); 
        
        Console.In.ReadLine();
    }

    private async Task<bool> ListenForClients()
    {
        // this right now only handles one client should expand to handle multiple
        var receiveTask = Receive();    
        SendRequestScan();
        var message = await receiveTask;

        ListenForResponsesFromClientCommands(message);
        
        for (int i = 0; i < 10; i++)
        {
            SendPowerOnRequest(message, i%2);
            
            await Task.Delay(2000);
        }
        
        return true;
    }

    private async void ListenForResponsesFromClientCommands(GoveeScanResponse message)
    {
        
        using (var udpClient = new UdpClient())
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 4002);
            udpClient.Client.Bind(ipEndPoint);
            do
            {
                var data = await udpClient.ReceiveAsync(CancellationToken.None);

                try
                {
                    var wrapper = JsonSerializer.Deserialize<GoveeWrapper>(data.Buffer);
                    
                    if (wrapper!.Message.Command != GoveeCommands.Status)
                    {
                        Console.Write(data.Buffer);
                    }
                    
                }
                catch (JsonException err)
                {
                    Console.WriteLine(err);
                }
            } while (true);
        }
    
    }

    private async Task<GoveeScanResponse> Receive()
    {
        GoveeScanResponse? message = null;
        using var udpClient = new UdpClient();
        
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 4002);
        udpClient.Client.Bind(ipEndPoint);
        do
        {
            var data = await udpClient.ReceiveAsync(CancellationToken.None);

            try
            {
                var wrapper = JsonSerializer.Deserialize<GoveeWrapper>(data.Buffer);
                if (wrapper.Message.Command != "scan")
                {
                    continue;
                }

                message = JsonSerializer.Deserialize<GoveeScanResponse>(wrapper.Message.Data);
            }
            catch (JsonException err)
            {
                Console.WriteLine(err);
            }
        } while (message is null);
        return message;
    }

    private void SendRequestScan()
    {
        using var udpClient = new UdpClient(4001);
        var endPoint = new IPEndPoint(IPAddress.Broadcast, 4001);
        
        var message = new GoveeWrapper<GoveeScanRequest>(new ());
        
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
        
    }
    private void SendPowerOnRequest(GoveeScanResponse response, int onOff)
    {
        using var udpClient = new UdpClient(4003);
        var endPoint = new IPEndPoint(IPAddress.Parse(response.Ip), 4003);
        // turns it on
        var message = new GoveeWrapper<GoveePowerRequest>(new (){OnOff = onOff});        
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
    }
    private void SendRequestStatus(GoveeScanResponse response)
    {
        using var udpClient = new UdpClient(4003);
        var endPoint = new IPEndPoint(IPAddress.Parse(response.Ip), 4003);
        
        var message = new GoveeWrapper<GoveeStatusRequest>(new ());
        
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
    }
   
}