using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        if (AppData.GameManager != null)
        {
            Destroy(gameObject);
            return;
        }

        AppData.GameManager = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayerDetected()
    {
        Debug.Log("Detected.");
    }
}

