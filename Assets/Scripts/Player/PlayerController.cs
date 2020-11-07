using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)] private float _movementSmoothStep = .05f;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerCrouchSpeed = 5f;
    [SerializeField] private float _playerClimbingSpeed = 5f;
    [SerializeField] private float _jumpForce = 500f;

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

    public Vector2 Velocity => _rb.velocity;
    public Vector2 Direction { get; private set; }
    public PlayerStates PlayerState => _playerState; 

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _bottomBound = _boxCollider.offset.y - (_boxCollider.size.y / 2f);

        _playerState = PlayerStates.Default;
        AppData.InputController.OnJump += Jump;
        AppData.InputController.OnDownKeyPressed += Crouch;
        AppData.InputController.OnDownKeyReleased += ResetPlayerStateFromCrouch;
    }

    void OnDestroy()
    {
        if (AppData.InputController != null)
        {
            AppData.InputController.OnJump -= Jump;
            AppData.InputController.OnDownKeyPressed -= Crouch;
            AppData.InputController.OnDownKeyReleased -= ResetPlayerStateFromCrouch;
        }
    }

    public void Update()
    {
        if (AppData.InputController.HorizontalMovement < 0)
            Direction = new Vector2(-1, 0);
        if (AppData.InputController.HorizontalMovement > 0)
            Direction = new Vector2(1, 0);

        CheckLadder();

        if (_playerState != PlayerStates.Climbing && _rb.velocity.y < -0.1) 
        {
           _playerState = PlayerStates.Falling;
        }

        if (_readyToClimb && AppData.InputController.VerticalMovement > 0) 
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
            case PlayerStates.Hidden:
            case PlayerStates.Crouch:
                //move = CrouchMovement();
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
        float horizontalMovement = AppData.InputController.HorizontalMovement * _playerSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, _rb.velocity.y);
        return velocity;
    }

    Vector2 ClimbingMovement() 
    {
        float verticalMovement = AppData.InputController.VerticalMovement * _playerClimbingSpeed;
        float horizontalMovement = AppData.InputController.HorizontalMovement * _playerClimbingSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, verticalMovement);
        return velocity;
    }

    Vector2 CrouchMovement()
    {
        float horizontalMovement = AppData.InputController.HorizontalMovement * _playerCrouchSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, _rb.velocity.y);
        return velocity;
    }

    void Crouch() 
    {
        if (_playerState == PlayerStates.Climbing || _playerState == PlayerStates.Falling) 
            return;

        if (_isGrounded && _playerState == PlayerStates.Default)
        {
            _playerState = PlayerStates.Crouch;
            RaycastHit2D hitInfo = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.5f), 0, Vector2.up, 0f, 1 << 9);
            if (hitInfo.collider != null) 
            {
                _playerState = PlayerStates.Hidden;
            }
            return;
        }
    }

    void ResetPlayerStateFromCrouch() 
    {
        if (_playerState == PlayerStates.Crouch || _playerState == PlayerStates.Hidden)
        {
            _playerState = PlayerStates.Default;
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
        //Collider2D ground = Physics2D.OverlapCircle(_groundCheckPosition.position, _groundCheckRadius, _groundLayer);
        RaycastHit2D groundHit = Physics2D.BoxCast(transform.position, new Vector2(1, 1f), 0, Vector2.down, 0.2f, 1 << 8);
        return groundHit.collider;
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
