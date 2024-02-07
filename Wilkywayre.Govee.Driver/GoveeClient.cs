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
     */
    
    
    public GoveeClient()
    {
        var client = ListenForClients(); 
        
        Console.In.ReadLine();
    }

    private async Task<GoveeClient> ListenForClients()
    {
        var receiveTask = Receive();    
        SendRequestScan();
        var message = await receiveTask;
        
        var wrappedMessage = JsonSerializer.Deserialize<GoveeWrapper>(message);
        
        return new ();
    }
    
    private async Task<string> Receive()
    {
        var udpClient = new UdpClient();
        var ipEndPoint = new IPEndPoint(IPAddress.Any, 4002);
        udpClient.Client.Bind(ipEndPoint);
        // listens for a response to the request scan message
        var data = await udpClient.ReceiveAsync(CancellationToken.None);
        var message = Encoding.Default.GetString(data.Buffer);
        return message;
    }

    private void SendRequestScan()
    {
        var udpClient = new UdpClient(4001);
        var endPoint = new IPEndPoint(IPAddress.Broadcast, 4001);
        
        var message = new GoveeWrapper()
        {
            msg = JsonSerializer.SerializeToElement(new GoveeRequestScan())
        };
        
        var requestScanMessage = Encoding.Default.GetBytes(JsonSerializer.Serialize(message));
        udpClient.Send(requestScanMessage, requestScanMessage.Length, endPoint);
    }
    
}