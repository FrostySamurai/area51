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
}

