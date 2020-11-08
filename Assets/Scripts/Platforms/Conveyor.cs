using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Conveyor : MonoBehaviour
{
    private SurfaceEffector2D _surfaceEffector = null;

    private List<Animator> _animators = null;

    private void Awake()
    {
        if (!TryGetComponent(out _surfaceEffector))
            Debug.LogError($"{name} is missing SurfaceEffector2D.");

        _animators = GetComponentsInChildren<Animator>(true).ToList();
    }

    public void ToggleEnabled(bool enabled)
    {
        if (_surfaceEffector != null)
            _surfaceEffector.enabled = enabled;

        _animators.ForEach(animator => animator.enabled = enabled);
    }
}

