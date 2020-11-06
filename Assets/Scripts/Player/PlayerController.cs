using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        None,
        Crouch,
        Hidden,
        Climbing
    }

    [SerializeField, Range(0, 0.5f)] private float _movementSmoothStep = .05f;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerCrouchSpeed = 5f;
    [SerializeField] private float _playerClimbingSpeed = 5f;
    [SerializeField] private float _jumpForce = 500f;
    [SerializeField] private float offset = 0;
    [SerializeField] private Transform _groundCheckPosition;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerState _playerState;  
    private Vector2 _velocity;

    // components
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    
    // flags
    private bool _isGrounded = true;
    private bool _jumpFromLadge = true;

    // controls
    private float _horizontalMovement = 0;
    private float _verticalMovement = 0;
    private bool _jump = false;
    private bool _isClimbing = false;

    //input controller
    private InputController _inputController;
    public InputController InputController 
    {
        get 
        {
            if (_inputController == null)
                _inputController = FindObjectOfType<InputController>();
            return _inputController;
        } 
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerState = PlayerState.None;
    }

    public void Update()
    {
        if (!_isGrounded && CheckGround())
        {
            _isGrounded = true;
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement() 
    {
        Vector2 move = Vector2.zero;
        switch (_playerState)
        {
            case PlayerState.None:
                move = HorizontalMovement();
                Jump();               
                break;
            case PlayerState.Crouch:
                CrouchMovement();
                break;
            case PlayerState.Climbing:
                move = VerticalMovement();
                break;
        }

        _rb.velocity = Vector2.SmoothDamp(_rb.velocity, move, ref _velocity, _movementSmoothStep);
    }

    Vector2 HorizontalMovement() 
    {
        float horizontalMovement = InputController.HorizontalMovement * _playerSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, _rb.velocity.y);
        return velocity;
    }

    Vector2 VerticalMovement() 
    {
        float verticalMovement = InputController.VerticalMovement * _playerClimbingSpeed;
        float horizontalMovement = InputController.HorizontalMovement * _playerSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, verticalMovement);
        return velocity;
    }

    void CrouchMovement() 
    {
        
    }

    void Jump() 
    {
        if (InputController.Jump && _isGrounded)
        {
            _isGrounded = false;
            InputController.Jump = false;
            _rb.AddForce(new Vector2(0f, _jumpForce));
        }
    }

    Collider2D CheckGround() 
    {
        Collider2D ground = Physics2D.OverlapCircle(_groundCheckPosition.position, _groundCheckRadius, _groundLayer);
        return ground;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder")) 
        {
            _playerState = PlayerState.Climbing;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _playerState = PlayerState.None;
    }

}
