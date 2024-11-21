using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private SimulationManagerUI _simulationManagerUI;
    [SerializeField] private ObjectDetector _objectDetector;
    [SerializeField] private AutonomousVehicle _autonomousVehicle;

    void OnEnable()
    {
        _simulationManagerUI.onPlayPressed += Play;
        _simulationManagerUI.onPausePressed += Pause;
        _simulationManagerUI.onResetPressed += Reset;
    }

    void OnDisable()
    {
        _simulationManagerUI.onPlayPressed -= Play;
        _simulationManagerUI.onPausePressed -= Pause;
        _simulationManagerUI.onResetPressed -= Reset;
    }

    private void Play()
    {
        _autonomousVehicle.Play();
        _objectDetector.IsOn = true;
        _simulationManagerUI.PlayMode();
    }

    private void Pause()
    {
        _autonomousVehicle.Pause();
        _objectDetector.IsOn = false;
        _simulationManagerUI.PauseMode();
    }

    private void Reset()
    {
        _autonomousVehicle.ResetState();
        _objectDetector.IsOn = false;
        _simulationManagerUI.ResetMode();
    }
}
