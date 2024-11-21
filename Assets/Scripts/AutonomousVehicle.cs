using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
class AutonomousVehicle : MonoBehaviour
{
    public bool IsPlaying { get; private set; }

    [SerializeField] private Transform _waypointContainer; // Container for the waypoints
    [SerializeField] private float _maxMotorTorque = 500f; // Maximum motor torque applied to the wheels
    [SerializeField] private float _maxSteerAngle = 30f;    // Maximum steering angle for front wheels
    [SerializeField] private float _waypointThreshold = 3f; // Distance threshold to switch to the next waypoint
    [SerializeField] private float _brakeTorque = 100000f;   // Stronger brake torque to stop the vehicle more effectively
    [Header("Wheels Colliders")]
    [SerializeField] private WheelCollider _frontLeftCollider;  // WheelColliders for the wheels
    [SerializeField] private WheelCollider _frontRightCollider;
    [SerializeField] private WheelCollider _rearLeftCollider;
    [SerializeField] private WheelCollider _rearRightCollider;
    [Header("Wheels Meshes")]
    [SerializeField] private Transform _frontLeftMesh;          // Wheel meshes for visual rotation
    [SerializeField] private Transform _frontRightMesh;
    [SerializeField] private Transform _rearLeftMesh;
    [SerializeField] private Transform _rearRightMesh;
    
    [Header("Loop Settings")]
    [SerializeField] private bool _isLoop = false;  
    

    private Rigidbody _rb;
    private Transform[] _waypoints;
    private int _currentWaypointIndex = 0;    // Tracks the current waypoint
    private bool _wasPaused = false;
    private bool _wasReset = false;
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private Vector3 _lastVelocity;
    private Vector3 _lastAngularVelocity;
    private float _lastMotorTorque;



    private void Start()
    {
        _rb = GetComponent<Rigidbody>();      // Ensure the vehicle has a Rigidbody

        _waypoints = _waypointContainer.GetComponentsInChildren<Transform>();

        _initialPosition = transform.position;
        _initialRotation = transform.rotation;

        IsPlaying = false;
    }

    private void FixedUpdate()
    {
        if (_wasReset && transform.position != _initialPosition)
        {
            transform.SetPositionAndRotation(_initialPosition, _initialRotation);
            _wasReset = false;
        }
        if (!IsPlaying) return;

        if (_waypoints.Length == 0) return;   // If no waypoints are set, return

        // Check if we have reached the last waypoint and stop or loop
        if (_currentWaypointIndex >= _waypoints.Length)
        {
            if (_isLoop)
            {
                LoopWaypoints();  // Loop back to the first waypoint
            }
            else
            {
                StopVehicle();    // Stop the vehicle at the last waypoint
            }
            return;
        }

        NavigateToWaypoint();                // Move vehicle towards the next waypoint
        UpdateWheelVisuals();                // Rotate the wheel meshes for visual effect
    }

    public void Play()
    {
        if (_wasPaused)
        {
            _rb.isKinematic = false;
            _rb.velocity = _lastVelocity;
            _rb.angularVelocity = _lastAngularVelocity;
            _rearLeftCollider.motorTorque = _lastMotorTorque;
            _rearRightCollider.motorTorque = _lastMotorTorque;
        }
        IsPlaying = true;
    }

    public void Pause()
    {
        IsPlaying = false;
        _wasPaused = true;
        
        _lastVelocity = _rb.velocity;
        _lastAngularVelocity = _rb.angularVelocity;
        _lastMotorTorque = _rearLeftCollider.motorTorque;

        _rb.isKinematic = true;

    }

    public void ResetState()
    {
        _currentWaypointIndex = 0;

        _rb.isKinematic = false;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rearLeftCollider.motorTorque = 0f;
        _rearRightCollider.motorTorque = 0f;
        _frontLeftCollider.steerAngle = 0f;
        _frontRightCollider.steerAngle = 0f;

        transform.SetPositionAndRotation(_initialPosition, _initialRotation);

        _wasPaused = false;
        IsPlaying = false;
        _wasReset = true;
    }

    void NavigateToWaypoint()
    {
        // Get the current waypoint
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];

        // Get the local velocity of the vehicle
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);

        // Steering
        float steerAngle = CalculateSteeringAngle(targetWaypoint.position);
        _frontLeftCollider.steerAngle = steerAngle;
        _frontRightCollider.steerAngle = steerAngle;

        // Adjust motor torque based on the forward velocity component
        float forwardVelocity = localVelocity.z; // Z is forward in local space
        float motorTorque = _maxMotorTorque;

     // Reduce torque if moving too fast
        motorTorque *= Mathf.Clamp01(1f - Mathf.Abs(forwardVelocity) / 20f);

        // Apply motor torque to rear wheels, scaled by forward speed
        _rearLeftCollider.motorTorque = motorTorque;
        _rearRightCollider.motorTorque = motorTorque;

        // Check if close enough to switch to the next waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < _waypointThreshold)
        {
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= _waypoints.Length && !_isLoop)
            {
                StopVehicle(); // Stop the vehicle if not looping
            }
        }

        // Apply braking if necessary (no braking in this case)
        ApplyBrakes(false);
    }

    float CalculateSteeringAngle(Vector3 targetPosition)
    {
        // Calculate direction to the target waypoint in local space
        Vector3 localTarget = transform.InverseTransformPoint(targetPosition);

        // Calculate the steering angle needed (limited by maxSteerAngle)
        float angle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        return Mathf.Clamp(angle, -_maxSteerAngle, _maxSteerAngle);
    }

    void ApplyBrakes(bool apply)
    {
        if (apply)
        {
            _rearLeftCollider.brakeTorque = _brakeTorque;
            _rearRightCollider.brakeTorque = _brakeTorque;
            _frontLeftCollider.brakeTorque = _brakeTorque;
            _frontRightCollider.brakeTorque = _brakeTorque;
        }
        else
        {
            _rearLeftCollider.brakeTorque = 0f;
            _rearRightCollider.brakeTorque = 0f;
            _frontLeftCollider.brakeTorque = 0f;
            _frontRightCollider.brakeTorque = 0f;
        }
    }

    void StopVehicle()
    {
        // Apply brakes to the vehicle
        ApplyBrakes(true);

        // Optionally, set motor torque to zero as well to stop the wheels from spinning
        _rearLeftCollider.motorTorque = 0f;
        _rearRightCollider.motorTorque = 0f;
    }

    void LoopWaypoints()
    {
        // When looping, reset to the first waypoint after reaching the last one
        _currentWaypointIndex = 0;  // Go back to the first waypoint

        // Apply brakes and then release them to continue movement
        ApplyBrakes(false);
    }

    void UpdateWheelVisuals()
    {
        // Rotate the wheel meshes based on the WheelColliders
        UpdateWheelMesh(_frontLeftCollider, _frontLeftMesh);
        UpdateWheelMesh(_frontRightCollider, _frontRightMesh);
        UpdateWheelMesh(_rearLeftCollider, _rearLeftMesh);
        UpdateWheelMesh(_rearRightCollider, _rearRightMesh);
    }

    void UpdateWheelMesh(WheelCollider collider, Transform mesh)
    {
        // Get the wheel collider's current position and rotation for the visual wheel
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        // Set the wheel mesh position and rotation to match the collider
        mesh.position = position;
        mesh.rotation = rotation;
    }
}
