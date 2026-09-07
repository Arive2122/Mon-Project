using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class LanDiscovery : MonoBehaviour
{
    public int discoveryPort = 47777;
    public string broadcastMessage = "PVP_LOBBY_HOST";

    UdpClient broadcastClient;
    UdpClient listenClient;
    bool isBroadcasting;
    bool isListening;

    public void StartBroadcasting()
    {
        if (isBroadcasting) return;

        broadcastClient = new UdpClient();
        broadcastClient.EnableBroadcast = true;
        isBroadcasting = true;

        InvokeRepeating(nameof(SendBroadcast), 0f, 1f);
    }

    void SendBroadcast()
    {
        if (broadcastClient == null) return;

        byte[] data = Encoding.UTF8.GetBytes(broadcastMessage);
        broadcastClient.Send(data, data.Length, new IPEndPoint(IPAddress.Broadcast, discoveryPort));
    }

    public void StopBroadcasting()
    {
        if (!isBroadcasting) return;

        CancelInvoke(nameof(SendBroadcast));
        broadcastClient?.Close();
        broadcastClient = null;
        isBroadcasting = false;
    }

    public void StartListening()
    {
        if (isListening) return;

        listenClient = new UdpClient(discoveryPort);
        listenClient.EnableBroadcast = true;
        isListening = true;
    }

    // Call every frame while listening. Returns the host's IP if found, otherwise null.
    public string CheckForHost()
    {
        if (listenClient == null || listenClient.Available == 0)
            return null;

        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = listenClient.Receive(ref remoteEP);
        string message = Encoding.UTF8.GetString(data);

        return message == broadcastMessage ? remoteEP.Address.ToString() : null;
    }

    public void StopListening()
    {
        if (!isListening) return;

        listenClient?.Close();
        listenClient = null;
        isListening = false;
    }

    void OnDestroy()
    {
        StopBroadcasting();
        StopListening();
    }
}