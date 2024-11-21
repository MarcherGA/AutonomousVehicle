using UnityEngine;
using System;

public abstract class UDPMessageObserver : MonoBehaviour
{
    [SerializeField] protected UDPReceiver _udpReceiver;

    protected void OnEnable()
    {
        // Listen to the UDPReceiver's data received event
        _udpReceiver.OnDataReceived += OnDataReceived;
    }
    
    protected void OnDisable()
    {
        _udpReceiver.OnDataReceived -= OnDataReceived;
    }
    

    protected virtual void OnDataReceived(string message)
    {
    }
}