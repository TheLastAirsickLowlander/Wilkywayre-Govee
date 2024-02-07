using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Wilkywayre.Govee.Driver.Model;

namespace Wilkywayre.Govee.Driver;

public class GoveeClient
{
    /* Wireshark Command
     * _ws.col.protocol == "UDP" && (ip.src == 192.168.1.2  ||  ip.src == 192.168.1.60) && udp.dstport != 3702
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
        
        return true;
    }
    
    private async Task<GoveeScanResponse> Receive()
    {
        GoveeScanResponse? message = null;
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
        }
        return message;
    }

    private void SendRequestScan()
    {
        var udpClient = new UdpClient(4001);
        var endPoint = new IPEndPoint(IPAddress.Broadcast, 4001);
        
        var message = new GoveeWrapper(GoveeScanRequest.Command ,JsonSerializer.SerializeToElement(new GoveeScanRequest()));
        
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
    }
    
}