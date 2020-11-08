using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplayer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _text = null;

    private Queue<string> _messagesToDisplay = new Queue<string>();
    private bool _isMessageDisplayed = false;
    private bool _loadMainMenu = false;

    public event Action OnMessagesShown = null;

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

    public void DisplayAfterword()
    {
        DisplayMessage("Hmmmmmmmmm, so Area51 was Santa's workshop all along. And the rumored aliens are, in reality, Santa's little helper elves!?");
        DisplayMessage("Maybe I should seriously check my facts and stop believing everything I see and sounds interesting.");
        DisplayMessage("Thanks for playing!");
        _loadMainMenu = true;
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
        print(_messagesToDisplay.Count);
        gameObject.SetActive(true);
    }

    public void ConfirmMessage()
    {
        if (AppData.GameManager.GameOver)
        {
            AppData.GameManager.RestartLevel();
            _isMessageDisplayed = false;
            gameObject.SetActive(false);
            return;
        }

        if (_messagesToDisplay.Count > 0)
        {
            _text.text = _messagesToDisplay.Dequeue();
            return;
        }

        _isMessageDisplayed = false;
        gameObject.SetActive(false);

        AppData.GameManager.ToggleControllers(true);
        OnMessagesShown?.Invoke();

        if (_loadMainMenu)
        {
            _loadMainMenu = false;
            AppData.SceneLoader.LoadScene(SceneName.MainMenu);
        }
    }
}

