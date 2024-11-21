using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ObjectDetector : MonoBehaviour
{
    public event Action<DetectedEntity> onEntityDetected;

    public bool IsOn { get; set; }

    [SerializeField] private LayerMask _detectionLayer;
    [SerializeField] private float _detectionUpdateFrequency = 0.1f;

    private Camera _sensorCamera;
    private HashSet<Collider> _detectedEntities = new HashSet<Collider>();
    private Collider[] _overlapResults = new Collider[100];  // For optimized detection, we pre-allocate an array for overlap results, Adjust size as needed
    private List<Collider> _detectedEntitiesToRemove = new List<Collider>();
    private Plane[] _cachedFrustumPlanes;  // Cached frustum planes
    // To track when the camera moves or rotates
    private Vector3 _lastCameraPosition;
    private Quaternion _lastCameraRotation;
    private float _timeSinceLastUpdate = 0f;

    private void Awake()
    {
        _sensorCamera = GetComponent<Camera>();
        _lastCameraPosition = _sensorCamera.transform.position;
        _lastCameraRotation = _sensorCamera.transform.rotation;

        IsOn = false;
    }

    private void Update()
    {
        if (!IsOn) return;
        _timeSinceLastUpdate += Time.deltaTime;

        // Only update detection after a certain interval to save performance
        if (_timeSinceLastUpdate >= _detectionUpdateFrequency)
        {
            DetectEntitiesInFrustum();
            _timeSinceLastUpdate = 0f;
        }
    }

    // Detect entities within the camera's frustum and proximity range
    private void DetectEntitiesInFrustum()
    {
        // Recalculate frustum planes only if the camera has moved or rotated significantly
        if (HasCameraMoved())
        {
            _cachedFrustumPlanes = GeometryUtility.CalculateFrustumPlanes(_sensorCamera);
            _lastCameraPosition = _sensorCamera.transform.position;
            _lastCameraRotation = _sensorCamera.transform.rotation;
        }

        _detectedEntitiesToRemove.Clear();
        // Check all already detected entities to see if they're still in the camera's frustum and visible
        foreach (var detectedCollider in _detectedEntities)
        {
            if (!IsObjectInFrustum(detectedCollider) || !IsVisible(detectedCollider.gameObject))
            {
                _detectedEntitiesToRemove.Add(detectedCollider);
            }
        }

        foreach (var colliderToRemove in _detectedEntitiesToRemove)
        {
            _detectedEntities.Remove(colliderToRemove);
        }


        // Now find all colliders within the detection radius (for potential new entities)
        Physics.OverlapSphereNonAlloc(_sensorCamera.transform.position, _sensorCamera.farClipPlane, _overlapResults, _detectionLayer);

        // Iterate through colliders and check if they are inside the camera's frustum and visible
        foreach (Collider col in _overlapResults)
        {
            if (col == null)
            {
                break;
            }
            // Only add the object to the HashSet if it's not already detected and is in view and visible
            if (!_detectedEntities.Contains(col) && IsObjectInFrustum(col) && IsVisible(col.gameObject))
            {
                _detectedEntities.Add(col);

                onEntityDetected?.Invoke(new DetectedEntity(col.tag, col.transform.position, Time.time));
            }
        }

    }

    private bool HasCameraMoved()
    {
        return (_sensorCamera.transform.position != _lastCameraPosition || _sensorCamera.transform.rotation != _lastCameraRotation);
    }

    private bool IsObjectInFrustum(Collider collider)
    {
        return GeometryUtility.TestPlanesAABB(_cachedFrustumPlanes, collider.bounds); // Check if the object's is in the camera's frustum
    }

     bool IsVisible(GameObject obj)
    {
        Vector3 directionToObj = obj.transform.position - _sensorCamera.transform.position;
        Ray rayToObj = new Ray(_sensorCamera.transform.position, directionToObj);

        RaycastHit hit;
        // Perform raycast to see if there's an obstacle between the camera and the object
        if (Physics.Raycast(rayToObj, out hit, _sensorCamera.farClipPlane))
        {
            if (hit.collider.gameObject == obj)
            {
                return true;
            }
        }

        return false;
    }
}


[System.Serializable]
public class DetectedEntity
{
    public string entityType {get; private set;}
    public Vector3 worldPosition {get; private set;}
    public float timeDetected {get; private set;}

    public DetectedEntity(string entityType, Vector3 worldPosition, float timeDetected)
    {
        this.entityType = entityType;
        this.worldPosition = worldPosition;
        this.timeDetected = timeDetected;
    }
}