using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)] private float _movementSmoothStep = .05f;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerCrouchSpeed = 5f;
    [SerializeField] private float _playerClimbingSpeed = 5f;
    [SerializeField] private float _playerPushingSpeed = 5f;
    [SerializeField] private float _jumpForce = 500f;

    [SerializeField] private PlayerStates _playerState;  
    private Vector2 _velocity;
    private float _bottomBound = 0;

    // components
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private Animator _animator;

    // flags
    private bool _isGrounded = true;
    private bool _readyToClimb = false;

    public Vector2 Velocity => _rb.velocity;
    public Vector2 Direction { get; private set; }
    public PlayerStates PlayerState { get { return _playerState; } set { _playerState = value; } }


    protected readonly int h_horizontalMovement = Animator.StringToHash("HorizontalMovement");
    protected readonly int h_VerticalMovement = Animator.StringToHash("VerticalMovement");
    protected readonly int h_jump = Animator.StringToHash("Jump");
    protected readonly int h_crouch = Animator.StringToHash("Crouch");
    protected readonly int h_climb = Animator.StringToHash("Climb");
    protected readonly int h_push = Animator.StringToHash("Push");

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _bottomBound = _boxCollider.offset.y - (_boxCollider.size.y / 2f);

        _playerState = PlayerStates.Default;
        AppData.InputController.OnJump += Jump;
        AppData.InputController.OnDownKey += Crouch;
        AppData.InputController.OnDownKeyReleased += ResetPlayerStateFromCrouch;
    }

    void OnDestroy()
    {
        if (AppData.InputController != null)
        {
            AppData.InputController.OnJump -= Jump;
            AppData.InputController.OnDownKey -= Crouch;
            AppData.InputController.OnDownKeyReleased -= ResetPlayerStateFromCrouch;
        }
    }

    public void Update()
    {
        CheckDirection();
        CheckLadder();

        if (_playerState != PlayerStates.Climbing && _rb.velocity.y < - 0.1f) 
        {
           _playerState = PlayerStates.Falling;
           _animator.SetBool(h_jump, true);
        }

        if (_readyToClimb && AppData.InputController.VerticalMovement > 0) 
        {
            _playerState = PlayerStates.Climbing;
            _readyToClimb = false;
            _animator.SetBool(h_jump, false);
        }

        if (_playerState != PlayerStates.Climbing && _rb.velocity.y >= 0.1) 
        {       
            if (_isGrounded = CheckGround())
            {
                _animator.SetBool(h_jump, false);
            }
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

        _animator.SetBool(h_push, false);
        _animator.SetBool(h_climb, false);
        _animator.SetBool(h_crouch, false);
        _animator.SetFloat(h_VerticalMovement, 0);

        switch (_playerState)
        {
            case PlayerStates.Falling:
                _isGrounded = CheckGround();
                move = HorizontalMovement();
                if (_isGrounded) 
                {
                    _playerState = PlayerStates.Default;
                    _animator.SetBool(h_jump, false);
                }              
                break;
            case PlayerStates.Default:
                move = HorizontalMovement();
                _animator.SetFloat(h_horizontalMovement, Mathf.Abs(AppData.InputController.HorizontalMovement));
                break;
            case PlayerStates.Pushing:
                move = PushingMovement();
                _animator.SetBool(h_push, true);
                break;
            case PlayerStates.Hidden:
            case PlayerStates.Crouch:
                //move = CrouchMovement();
                _animator.SetBool(h_crouch, true);
                break;
            case PlayerStates.Climbing:
                _rb.gravityScale = 0f;
                move = ClimbingMovement();
                _animator.SetFloat(h_VerticalMovement, Mathf.Abs(AppData.InputController.VerticalMovement));
                _animator.SetBool(h_climb, true) ;
                break;
        }

        _rb.velocity = Vector2.SmoothDamp(_rb.velocity, move, ref _velocity, _movementSmoothStep);
    }

    void CheckDirection() 
    {
        if (_playerState == PlayerStates.Pushing)
            return;

        if (AppData.InputController.HorizontalMovement < 0)
        {
            Direction = new Vector2(-1, 0);
            _spriteRenderer.flipX = true;
        }
        if (AppData.InputController.HorizontalMovement > 0)
        {
            Direction = new Vector2(1, 0);
            _spriteRenderer.flipX = false;
        }
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

    Vector2 PushingMovement()
    {
        float horizontalMovement = AppData.InputController.HorizontalMovement * _playerPushingSpeed;
        Vector2 velocity = new Vector2(horizontalMovement, 0);
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
            _animator.SetBool(h_jump, true);
        }
    }

    Collider2D CheckGround() 
    {
        //Collider2D ground = Physics2D.OverlapCircle(_groundCheckPosition.position, _groundCheckRadius, _groundLayer);
        RaycastHit2D groundHit = Physics2D.BoxCast(transform.position, new Vector2(1, 1f), 0, Vector2.down, 0.1f, 1 << 8);
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

        if(hitInfo.collider == null) 
        {
            if (_playerState == PlayerStates.Climbing)
            {
                _playerState = PlayerStates.Default;
            }
        }   
    }
}
