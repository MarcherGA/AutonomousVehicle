using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

public class MessageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject _messageBoxPrefab;  // Message box prefab
    [SerializeField] private Transform _contentPanel;  // Content panel inside the scroll view

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>(); // Thread-safe queue

    private void Update()
    {
        // Process all queued messages
        while (messageQueue.TryDequeue(out string message))
        {
            CreateMessageBox(message);
        }
    }

    public void EnqueueMessage(string message)
    {
        messageQueue.Enqueue(message);
    }

    private void CreateMessageBox(string message)
    {
        // Instantiate the message box and set its text
        GameObject messageBox = Instantiate(_messageBoxPrefab, _contentPanel);
        messageBox.GetComponentInChildren<TMP_Text>().text = message;
    }

    public void ClearMessages()
    {
        foreach (Transform child in _contentPanel)
        {
            Destroy(child.gameObject);
        }
    }


}
