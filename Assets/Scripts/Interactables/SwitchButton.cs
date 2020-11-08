using UnityEngine;
using UnityEngine.Events;

public class SwitchButton : MonoBehaviour
{
    [SerializeField] private bool _oneShot = false;

    private Collider2D _collider = null;
    private AudioSource _audioSource = null;
    private InputController _inputController = null;
    private Animator _animator = null;
    private bool _isTurnedOn = false;

    public UnityEvent OnTurnOn = null;
    public UnityEvent OnTurnOff = null;

    private bool _isSwitching = false;

    private void Awake()
    {
        if (!TryGetComponent(out _collider))
            Debug.LogError($"{name} is missing collider.");
        else if (!_collider.isTrigger)
            Debug.LogWarning($"Collider on {name} should be set to trigger.");

        if (!TryGetComponent(out _audioSource))
            Debug.LogWarning($"{name} is missing an audio source.");

        if (!TryGetComponent(out _animator))
            Debug.LogError($"{name} is missing animator.");

        _inputController = FindObjectOfType<InputController>();
        if (_inputController == null)
            Debug.LogError($"Input controller is missing.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        if (_inputController != null)
            _inputController.OnInteractionPressed += Interact;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player")
            return;

        if (_inputController != null)
            _inputController.OnInteractionPressed -= Interact;
    }

    private void OnDestroy()
    {
        if (_inputController != null)
            _inputController.OnInteractionPressed -= Interact;
    }

    public void SwitchEnd() =>
        _isSwitching = false;

    private void Interact()
    {
        if (_isSwitching)
            return;

        if (_oneShot && _isTurnedOn)
            return;

        if (_audioSource != null)
            _audioSource.Play();

        if (!_isTurnedOn)
            OnTurnOn?.Invoke();
        else
            OnTurnOff?.Invoke();

        _isTurnedOn = !_isTurnedOn;
        _animator.SetTrigger("Switch");
        _isSwitching = true;
    }
}

