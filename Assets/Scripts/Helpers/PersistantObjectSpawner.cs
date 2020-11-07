using UnityEngine;

public class PersistantObjectSpawner : MonoBehaviour
{
    private static bool _spawned = false;

    [SerializeField] private GameObject _persistantObjectPrefab = null;
    [SerializeField] private GameObject _storyTellingCanvasPrefab = null;

    private void Awake()
    {
        if (_spawned)
            return;

        GameObject persistantObject = Instantiate(_persistantObjectPrefab);
        DontDestroyOnLoad(persistantObject);
        GameObject storyTellingCanvas = Instantiate(_storyTellingCanvasPrefab);
        DontDestroyOnLoad(storyTellingCanvas);
        _spawned = true;
    }
}

