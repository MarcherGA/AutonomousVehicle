using UnityEngine;
using System;

public class UDPDetectedEntityObserver : UDPMessageObserver
{
    [SerializeField] private MessageDisplay _messageDisplay;
    

    protected override void OnDataReceived(string message)
    {
        try
        {
            DetectedEntityMessage detectedEntityMessage = JsonUtility.FromJson<DetectedEntityMessage>(message);
            string result = $"Detected entity: {detectedEntityMessage.entityType} Position: {detectedEntityMessage.entityPosition} Time Detected: {detectedEntityMessage.detectionTime} Sensor Position: {detectedEntityMessage.sensorPosition}";
            _messageDisplay.EnqueueMessage(result);
        }
        catch (Exception ex)
        {
        }
    }
}