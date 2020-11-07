using UnityEngine;


public class InputController : MonoBehaviour
{
    public float HorizontalMovement { get; private set; }
    public float VerticalMovement { get; private set; }

    public event System.Action OnJump;
    public event System.Action OnDownKeyPressed;
    public event System.Action OnDownKeyReleased;
    public event System.Action OnInteraction;

    void Awake() 
    {
        if (AppData.InputController != null)
        {
            Destroy(gameObject);
            return;
        }

        AppData.InputController = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        HorizontalMovement = 0;
        VerticalMovement = 0;
    }

    private void Update()
    {
        HorizontalMovement = Input.GetAxis("Horizontal");
        VerticalMovement = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            OnJump?.Invoke();
        }

        if (Input.GetKey(KeyCode.S))
        {
            OnDownKeyPressed?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            OnDownKeyReleased?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteraction?.Invoke();
        }
    }
}
