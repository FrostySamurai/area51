using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool LevelIsDone = false;

    private void Awake()
    {
        AppData.GameManager = this;
    }

    public void PlayerDetected()
    {
        Debug.Log("Detected.");
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

