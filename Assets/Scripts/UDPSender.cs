using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class UDPSender : MonoBehaviour
{
    [SerializeField] private ConfigLoader _configLoader;
    private UdpClient _udpClient;
    private IPEndPoint _remoteEndPoint;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        var config = _configLoader.ConfigData;
        _udpClient = new UdpClient();
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(config.ipAddress), config.udpPort);
    }

    public void SendData(string message)
    {
        if (_udpClient == null)
        {
            Initialize();
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        _udpClient.Send(data, data.Length, _remoteEndPoint);
    }

    public void Close()
    {
        _udpClient?.Close();
    }

    void OnDestroy()
    {
        Close();
    }
}
