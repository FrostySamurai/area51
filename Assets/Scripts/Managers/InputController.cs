using UnityEngine;


public class InputController : MonoBehaviour
{
    public float HorizontalMovement { get; private set; }
    public float VerticalMovement { get; private set; }

    public event System.Action OnJump;
    public event System.Action OnDownKey;
    public event System.Action OnDownKeyReleased;
    public event System.Action OnInteractionPressed;
    public event System.Action OnInteraction;
    public event System.Action OnInteractionReleased;
    
    public bool IsEnabled { get; set; }

    void Awake() 
    {
        AppData.InputController = this;
        IsEnabled = true;
    }

    private void Start()
    {
        HorizontalMovement = 0;
        VerticalMovement = 0;
    }

    private void OnDisable()
    {
        HorizontalMovement = 0;
        VerticalMovement = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            OnInteractionPressed?.Invoke();
        }

        if (!IsEnabled)
            return;

        HorizontalMovement = Input.GetAxis("Horizontal");
        VerticalMovement = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.J))
        {
            OnJump?.Invoke();
        }

        if (Input.GetKey(KeyCode.S))
        {
            OnDownKey?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            OnDownKeyReleased?.Invoke();
        }

        if (Input.GetKey(KeyCode.K))
        {
            OnInteraction?.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            OnInteractionReleased?.Invoke();
        }
    }
}
