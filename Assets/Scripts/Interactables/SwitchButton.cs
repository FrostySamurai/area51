using UnityEngine;
using UnityEngine.Events;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private bool _oneShot = false;

    private Collider2D _collider = null;
    private InputController _inputController = null;
    private bool _isTurnedOn = false;

    public UnityEvent OnTurnOn = null;
    public UnityEvent OnTurnOff = null;

    private void Awake()
    {
        if (!TryGetComponent(out _collider))
            Debug.LogError($"{name} is missing collider.");
        else if (!_collider.isTrigger)
            Debug.LogWarning($"Collider on {name} should be set to trigger.");

        _inputController = FindObjectOfType<InputController>();
        if (_inputController == null)
            Debug.LogError($"Input controller is missing.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_inputController != null)
            _inputController.OnInteractionPressed += Interact;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_inputController != null)
            _inputController.OnInteractionPressed -= Interact;
    }

    private void Interact()
    {
        if (_oneShot && _isTurnedOn)
            return;

        if (!_isTurnedOn)
            OnTurnOn?.Invoke();
        else
            OnTurnOff?.Invoke();

        _isTurnedOn = !_isTurnedOn;
    }
}

