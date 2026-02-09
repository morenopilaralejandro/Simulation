using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DataTransferDiscoveryListener : MonoBehaviour
{
    private UdpClient udp;
    private const int DISCOVERY_PORT = 7777;

    public System.Action<DataTransferDiscoveryPacket> OnServerFound;

    public void StartListening()
    {
        udp = new UdpClient(DISCOVERY_PORT);
        Task.Run(ListenLoop);
    }

    async Task ListenLoop()
    {
        while (true)
        {
            UdpReceiveResult result = await udp.ReceiveAsync();
            string json = Encoding.UTF8.GetString(result.Buffer);
            DataTransferDiscoveryPacket packet =
                JsonUtility.FromJson<DataTransferDiscoveryPacket>(json);

            OnServerFound?.Invoke(packet);
        }
    }

    public void StopListening()
    {
        udp?.Close();
    }
}
