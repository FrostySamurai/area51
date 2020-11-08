using UnityEngine;
using UnityEngine.Events;

public class TimerEvent : MonoBehaviour
{
    [SerializeField] private float _timerLength = 3f;

    public UnityEvent OnTimer = null;

    private void Start()
    {
        StartCoroutine(RunTimer());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(_timerLength);

        OnTimer?.Invoke();

        StartCoroutine(RunTimer());
    }
}

