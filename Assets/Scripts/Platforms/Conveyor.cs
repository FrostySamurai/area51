using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private SurfaceEffector2D _surfaceEffector = null;

    private void Awake()
    {
        if (!TryGetComponent(out _surfaceEffector))
            Debug.LogError($"{name} is missing SurfaceEffector2D.");
    }

    public void ToggleEnabled(bool enabled)
    {
        if (_surfaceEffector != null)
            _surfaceEffector.enabled = enabled;

        // MH: handle animations
    }
}

