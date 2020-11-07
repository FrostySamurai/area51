using UnityEngine;


public class InputController : MonoBehaviour
{
    public float HorizontalMovement { get; private set; }
    public float VerticalMovement { get; private set; }

    public event System.Action OnJump;
    public event System.Action OnCrouch;
    public event System.Action OnInteraction;

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

        if (Input.GetKeyDown(KeyCode.C))
        {
            OnCrouch?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            OnInteraction?.Invoke();
        }
    }
}
