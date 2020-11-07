using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent OnTriggerEnter = null;

    private void Awake()
    {
        if (!TryGetComponent(out Collider2D collider))
            Debug.LogError($"{name} is missing collider.");
        else if (collider.isTrigger == false)
            Debug.LogWarning($"{name}'s collider should be set to trigger.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerEnter?.Invoke();
    }
}

