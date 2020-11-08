using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class Gate : MonoBehaviour
{
    [SerializeField] private SceneName scene;

    private bool _isOpen = false;
    private Animator _animator = null;
    private AudioSource _audioSource = null;

    private void Awake()
    {
        if (!TryGetComponent(out _animator))
            Debug.LogError($"{name} is missing Animator.");

        if (!TryGetComponent(out _audioSource))
            Debug.LogError($"{name} is missing AudioSource.");
    }

    public void Open()
    {
        if (_isOpen)
            return;

        _isOpen = true;
        _animator.SetTrigger("Open");
        _audioSource.Play();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            Debug.Log(collision.gameObject.layer);
            if (_isOpen && AppData.InputController.VerticalMovement > 0)
            {
                AppData.SceneLoader.LoadScene(scene);
            }
        }
    }
}
