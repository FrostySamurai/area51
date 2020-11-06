using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Range(0, 0.5f)] private float _movementSmoothStep = .05f;
    [SerializeField] private float _playerSpeed = 5f;
    [SerializeField] private float _playerClimbingSpeed = 5f;
    [SerializeField] private float _jumpForce = 500f;
    [SerializeField] private float offset = 0;
    [SerializeField] private Transform _groundCheckPosition;
    [SerializeField] private float _groundCheckRadius;
    [SerializeField] private LayerMask _groundLayer;
    
    private Vector2 _velocity;


    // components
    private Rigidbody2D _rb;
    
    // flags
    private bool _isGrounded = true;
    
    // controls
    private float _horizontalMovement = 0;
    private float _verticalMovement = 0;
    private bool _jump = false;
    private bool _isClimbing = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _jump = true;          
        }

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
        Vector2 targetVelocity = Vector2.zero;
        if (_isClimbing) 
        {
            targetVelocity = new Vector2(_horizontalMovement * _playerSpeed, _verticalMovement * _playerClimbingSpeed);
            _rb.gravityScale = 0;
        } 
        else 
        {
            targetVelocity = new Vector2(_horizontalMovement * _playerSpeed, _rb.velocity.y);
            _rb.gravityScale = 1;
        }

        if (_jump && _isGrounded)
        {
            _isGrounded = false;
            _jump = false;
            _rb.AddForce(new Vector2(0f, _jumpForce));
        }

        _rb.velocity = Vector2.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, _movementSmoothStep);
        
    }


    Collider2D CheckGround() 
    {
        Collider2D ground = Physics2D.OverlapCircle(_groundCheckPosition.position, _groundCheckRadius, _groundLayer);
        return ground;
    }      
}
