using System;
using UnityEngine;
using UnityEngine.Events;

public class TimerButton : MonoBehaviour
{
    [SerializeField] private float _timerLength = 2f;

    private Collider2D _collider = null;
    private InputController _inputController = null;

    public UnityEvent OnTimerStart = null;
    public UnityEvent OnTimerEnd = null;

    public float TimeLeft { get; private set; }

    private void Awake()
    {
        TimeLeft = -1f;

        if (!TryGetComponent(out _collider))
            Debug.LogError($"{name} is missing collider.");
        else if (!_collider.isTrigger)
            Debug.LogWarning($"Collider on {name} should be set to trigger.");

        _inputController = FindObjectOfType<InputController>();
        if (_inputController == null)
            Debug.LogError($"Input controller is missing.");
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (TimeLeft <= 0f)
            return;

        TimeLeft -= Time.deltaTime;

        if (TimeLeft > 0f)
            return;

        OnTimerEnd?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_inputController != null)
            _inputController.OnInteraction += Interact;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_inputController != null)
            _inputController.OnInteraction -= Interact;
    }

    private void Interact()
    {
        if (TimeLeft > 0f)
        {
            TimeLeft = _timerLength;
            return;
        }

        TimeLeft = _timerLength;
        OnTimerStart?.Invoke();
    }
}

