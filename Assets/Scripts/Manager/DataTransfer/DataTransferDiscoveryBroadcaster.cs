using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class DataTransferDiscoveryBroadcaster : MonoBehaviour
{
    private UdpClient udp;
    private IPEndPoint endPoint;
    private const int DISCOVERY_PORT = 7777;

    public void StartBroadcast(int savePort)
    {
        udp = new UdpClient();
        udp.EnableBroadcast = true;
        endPoint = new IPEndPoint(IPAddress.Broadcast, DISCOVERY_PORT);

        InvokeRepeating(nameof(Broadcast), 0f, 1.5f);

        discoveryPacket = new DataTransferDiscoveryPacket
        {
            deviceName = SystemInfo.deviceName,
            ip = GetLocalIp(),
            port = savePort
        };
    }

    DataTransferDiscoveryPacket discoveryPacket;

    void Broadcast()
    {
        string json = JsonUtility.ToJson(discoveryPacket);
        byte[] data = Encoding.UTF8.GetBytes(json);
        udp.Send(data, data.Length, endPoint);
    }

    public void StopBroadcast()
    {
        CancelInvoke();
        udp?.Close();
    }

    private static string GetLocalIp()
    {
        try
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            return ((IPEndPoint)socket.LocalEndPoint).Address.ToString();
        }
        catch
        {
            return "127.0.0.1";
        }
    }
}
