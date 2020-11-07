using UnityEngine;

public class PersistantObjectSpawner : MonoBehaviour
{
    private static bool _spawned = false;

    [SerializeField] private GameObject _persistantObjectPrefab = null;

    private void Awake()
    {
        if (_spawned)
            return;

        GameObject persistantObject = Instantiate(_persistantObjectPrefab);
        DontDestroyOnLoad(persistantObject);
        _spawned = true;
    }
}

