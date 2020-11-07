using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        AppData.GameManager.StartGame();
    }

    public void Quit()
    {
        AppData.GameManager.QuitApplication();
    }
}

