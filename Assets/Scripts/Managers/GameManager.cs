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
        ToggleControllers(true);
        AppData.SceneLoader.RestartLevel();
        StartCoroutine(WaitForFrame());
    }

    private System.Collections.IEnumerator WaitForFrame()
    {
        yield return null;
        GameOver = false;
    }

    public void ToggleControllers(bool enabled)
    {
        AppData.InputController.IsEnabled = enabled;
        PlayerController player = null;
        if (player = FindObjectOfType<PlayerController>(true))
        {
            player.gameObject.GetComponent<Animator>().enabled = enabled;
            player.enabled = enabled;
        }
        foreach (EnemyController enemy in FindObjectsOfType<EnemyController>(true))
            enemy.enabled = enabled;
    }

    public void SetLevelIsDone(bool value) 
    {
        LevelIsDone = value;
    }

    public void StartGame()
    {
        AppData.SceneLoader.LoadScene(SceneName.Prologue);
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

