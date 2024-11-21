using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPReceiver : MonoBehaviour
{
    public event Action<string> OnDataReceived;

    [SerializeField] private ConfigLoader _configLoader;

    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private int listenPort = 8080; // Default port in case no config is provided

    public void Initialize()
    {
        listenPort = _configLoader?.ConfigData?.udpPort ?? listenPort;
    }

    private void Start()
    {
        udpClient = new UdpClient(listenPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        Debug.Log($"Listening for UDP messages on port {listenPort}");
        BeginReceive();
    }

    private void BeginReceive()
    {
        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnReceive(IAsyncResult result)
    {
        byte[] data = udpClient.EndReceive(result, ref remoteEndPoint);
        string message = Encoding.UTF8.GetString(data);

        OnDataReceived?.Invoke(message);

        BeginReceive();
    }

    private void OnApplicationQuit()
    {
        udpClient?.Close();
    }
}
