using System.Collections.Generic;
using UnityEngine;

public class MessageDisplayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _text = null;

    private Queue<string> _messagesToDisplay = new Queue<string>();
    private bool _isMessageDisplayed = false;

    private void Awake()
    {
        AppData.MessageDisplayer = this;
        AppData.InputController.OnInteractionPressed += ConfirmMessage;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (AppData.InputController != null)
            AppData.InputController.OnInteractionPressed -= ConfirmMessage;
    }

    public void DisplayMessage(string message)
    {
        if (_isMessageDisplayed)
        {
            _messagesToDisplay.Enqueue(message);
            return;
        }

        AppData.GameManager.ToggleControllers(false);

        _text.text = message;
        _isMessageDisplayed = true;

        gameObject.SetActive(true);
    }

    public void ConfirmMessage()
    {
        if (_messagesToDisplay.Count > 0)
        {
            _text.text = _messagesToDisplay.Dequeue();
            return;
        }

        _isMessageDisplayed = false;
        gameObject.SetActive(false);

        AppData.GameManager.ToggleControllers(true);
    }
}

