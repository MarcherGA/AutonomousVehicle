using UnityEngine;

public class DetectionObserver : MonoBehaviour
{
    [SerializeField] private ObjectDetector _objectDetector;
    [SerializeField] private UDPSender _udpSender;

    private void Start()
    {
        if (_objectDetector != null)
        {
            _objectDetector.onEntityDetected += HandleEntityDetected;
        }
    }

    private void HandleEntityDetected(DetectedEntity detectedEntity)
    {
        // Get the camera sensor position
        Vector3 sensorPosition = _objectDetector.transform.position;

        DetectedEntityMessage detectedEntityMessage = new DetectedEntityMessage(detectedEntity.entityType, detectedEntity.worldPosition, detectedEntity.timeDetected, sensorPosition);
        string message = JsonUtility.ToJson(detectedEntityMessage);

        // Send the data using the UDPSender
        _udpSender.SendData(message);
    }

    private void OnDestroy()
    {
        if (_objectDetector != null)
        {
            _objectDetector.onEntityDetected -= HandleEntityDetected;
        }
    }
}

[System.Serializable]
public class DetectedEntityMessage
{
    public string entityType;
    public Vector3 entityPosition;
    public float detectionTime;
    public Vector3 sensorPosition;

    public DetectedEntityMessage(string entityType, Vector3 entityPosition, float detectionTime, Vector3 sensorPosition)
    {
        this.entityType = entityType;
        this.entityPosition = entityPosition;
        this.detectionTime = detectionTime;
        this.sensorPosition = sensorPosition;
    }
}