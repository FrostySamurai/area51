using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)] private float _movementSmoothStep = .05f;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerCrouchSpeed = 5f;
    [SerializeField] private float _playerClimbingSpeed = 5f;
    [SerializeField] private float _jumpForce = 500f;

    [SerializeField] private Transform _groundCheckPosition;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;

    [SerializeField] private PlayerStates _playerState;  
    private Vector2 _velocity;
    private float _bottomBound = 0;
    // components
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    
    // flags
    private bool _isGrounded = true;
    private bool _readyToClimb = false;

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

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _bottomBound = _boxCollider.offset.y - (_boxCollider.size.y / 2f);

        _playerState = PlayerStates.Default;
        InputController.OnJump += Jump;
        InputController.OnCrouch += Crouch;
    }

    void OnDestroy()
    {
        if (InputController != null)
        {
            InputController.OnJump -= Jump;
            InputController.OnCrouch -= Crouch;
        }
    }

    public void Update()
    {
        CheckLadder();

        if (_rb.velocity.y < 0) 
        {
           _playerState = PlayerStates.Falling;
        }

        if (_readyToClimb && InputController.VerticalMovement > 0) 
        {
            _playerState = PlayerStates.Climbing;
            _readyToClimb = false;
        }
       
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement() 
    {
        Vector2 move = Vector2.zero;
        _rb.gravityScale = 1f;
        switch (_playerState)
        {
            case PlayerStates.Falling:
                _isGrounded = CheckGround();
                move = HorizontalMovement();
                if (_isGrounded)
                    _playerState = PlayerStates.Default;
                
                break;
            case PlayerStates.Default:
                move = HorizontalMovement();
                break;
            case PlayerStates.Crouch:
                move = CrouchMovement();
                break;
            case PlayerStates.Climbing:
                _rb.gravityScale = 0f;
                move = ClimbingMovement();
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

    Vector2 ClimbingMovement() 
    {
        float verticalMovement = InputController.VerticalMovement * _playerClimbingSpeed;
        float horizontalMovement = InputController.HorizontalMovement * _playerClimbingSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, verticalMovement);
        return velocity;
    }

    Vector2 CrouchMovement()
    {
        float horizontalMovement = InputController.HorizontalMovement * _playerCrouchSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, _rb.velocity.y);
        return velocity;
    }

    void Crouch() 
    {
        if (_isGrounded && _playerState == PlayerStates.Default)
        {
            _playerState = PlayerStates.Crouch;
            return;
        }

        if (_playerState == PlayerStates.Crouch)
        {
            _playerState = PlayerStates.Default;
            return;
        }
    }

    void Jump() 
    {
        if (_isGrounded && _playerState == PlayerStates.Default)
        {
            _isGrounded = false;
            _rb.AddForce(new Vector2(0f, _jumpForce));
        }
    }

    Collider2D CheckGround() 
    {
        Collider2D ground = Physics2D.OverlapCircle(_groundCheckPosition.position, _groundCheckRadius, _groundLayer);
        return ground;
    }

    void CheckLadder() 
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position + Vector3.up * _bottomBound, Vector2.up, 1f, ~(1 << 10));
        if (hitInfo.collider && hitInfo.collider.gameObject.GetComponent<Ladder>())
        {
            if (_playerState != PlayerStates.Climbing)
            {
                _readyToClimb = true;
            }        
        }
        else 
        {
            if (_playerState == PlayerStates.Climbing)
            {
                _playerState = PlayerStates.Default;
            }
        }   
    }
}
