using System;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManagerUI : MonoBehaviour
{   
    public Action onPlayPressed;
    public Action onPausePressed;
    public Action onResetPressed;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Button _resetButton;

    private void Start()
    {
        _playButton.onClick.AddListener(onPlayPressed.Invoke);
        _pauseButton.onClick.AddListener(onPausePressed.Invoke);
        _resetButton.onClick.AddListener(onResetPressed.Invoke);
    }

    private void OnDestroy()
    {
        if (_playButton == null || _pauseButton == null || _resetButton == null || onPlayPressed == null || onPausePressed == null || onResetPressed == null)
        {
            return;
        }
        _playButton.onClick.RemoveListener(onPlayPressed.Invoke);
        _pauseButton.onClick.RemoveListener(onPausePressed.Invoke);
        _resetButton.onClick.RemoveListener(onResetPressed.Invoke);
    }


    public void PlayMode()
    {
        _playButton.gameObject.SetActive(false);
        _pauseButton.gameObject.SetActive(true);
        _resetButton.gameObject.SetActive(false);
    }

    public void PauseMode()
    {
        _playButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
        _resetButton.gameObject.SetActive(true);
    }

    public void ResetMode()
    {    
        _playButton.gameObject.SetActive(true);
        _pauseButton.gameObject.SetActive(false);
        _resetButton.gameObject.SetActive(false);
    }

}
