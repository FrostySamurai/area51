using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool LevelIsDone = false;
    public bool GameOver { get; private set; }

    private void Awake()
    {
        AppData.GameManager = this;
        GameOver = false;
    }

    public void PlayerDetected()
    {
        GameOver = true;
        AppData.MessageDisplayer.DisplayMessage("You've been found!");
    }

    public void RestartLevel()
    {
        GameOver = false;
        ToggleControllers(true);
        AppData.SceneLoader.RestartLevel();
    }

    public void ToggleControllers(bool enabled)
    {
        AppData.InputController.IsEnabled = enabled;
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>(true))
            enemy.enabled = enabled;
    }

    public void StartGame()
    {
        AppData.SceneLoader.LoadScene(SceneName.Level1);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

