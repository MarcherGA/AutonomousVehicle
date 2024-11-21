using UnityEngine;

public class ConfigLoader : MonoBehaviour
{
    public ConfigData ConfigData => _configData;
    [SerializeField] private TextAsset _configFile;// Set in Unity Inspector
    private ConfigData _configData;

    private void Awake()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        if (_configFile != null)
        {
            _configData = JsonUtility.FromJson<ConfigData>(_configFile.text);
        }
        else
        {
            Debug.LogError("Config file not selected");
        }
    }

}

