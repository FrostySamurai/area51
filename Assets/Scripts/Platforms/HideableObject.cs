using UnityEngine;

public class HideableObject : MonoBehaviour
{
    [SerializeField, Range(0,1)] float _pushOffset = 0.2f;
    [SerializeField, Range(0, 1)] float _jumpLowerOffset = 0.5f;
    [SerializeField, Range(1, 5)] float _jumpHigherOffset = 2f;
    [SerializeField] bool _jumpAble = false;
    [SerializeField] bool _pushAble = false;

    private int _groundLayerMask = 8;
    private int _hideableLayerMask = 9;
    private bool _pushing = false;
    private bool _onTop = false;
    private bool _registerPush = false;
    private Vector2 _pushingDirection = Vector2.zero;
    private bool _downKeyPressed = false;
    private PlayerController _player;

    private Rigidbody2D _rb;

    private void Start()
    {
        AppData.InputController.OnDownKeyPressed += DownKeyPressed;
        AppData.InputController.OnDownKeyReleased += DownKeyReleased;
        _rb = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void OnDestroy()
    {
        if (AppData.InputController != null)
        {
            AppData.InputController.OnDownKeyPressed -= DownKeyPressed;
            AppData.InputController.OnDownKeyReleased -= DownKeyReleased;
        }
    }

    private void Update()
    {
        if (_jumpAble)
            gameObject.layer = JumpCollider();

        if (_pushAble)
            PushCollider();
    }

    private void FixedUpdate()
    {
        if (_pushing) 
        {
            Pushing();
        }
    }

    void DownKeyPressed() => _downKeyPressed = true;
    void DownKeyReleased() => _downKeyPressed = false;

    int JumpCollider() 
    {
        RaycastHit2D firstHit = Physics2D.BoxCast(transform.position + Vector3.up * _jumpLowerOffset, new Vector2(1, 1f), 0, Vector2.up, 0.1f, 1 << 10);
        RaycastHit2D secondHit = Physics2D.BoxCast(transform.position + Vector3.up * _jumpHigherOffset, new Vector2(1, 1f), 0, Vector2.up, 0.1f, 1 << 10);
        if (firstHit.collider != null && secondHit.collider != null)
        {
            _onTop = true;
            if (_downKeyPressed) 
            {
                _onTop = false;
                return _hideableLayerMask;
            }
            return _groundLayerMask;
        }
        _onTop = false;
        return _pushing ? _groundLayerMask : _hideableLayerMask;
    }

    void PushCollider() 
    {
        if(!_pushing)
            _pushingDirection = _player.Direction;
        RaycastHit2D firstHit = Physics2D.BoxCast(transform.position, new Vector2(1.1f, 0.8f), 0, -_pushingDirection, _pushOffset, 1 << 10);
        RaycastHit2D secondHit = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0, -_pushingDirection, 0.01f, 1 << 10);

        if (!_registerPush && firstHit.collider != null && secondHit.collider == null)
        {
            AppData.InputController.OnInteraction += Push;
            _registerPush = true;
            _player = firstHit.collider.GetComponent<PlayerController>();
        }

        if (_registerPush && (firstHit.collider == null || secondHit.collider != null))
        {
            AppData.InputController.OnInteraction -= Push;
            _registerPush = false;
            ResetProperties();
        }
    }

    void Push() 
    {
        _pushing = !_pushing;
        if (_pushing)
        {
            gameObject.layer = _groundLayerMask;
            _rb.sharedMaterial = _player.GetComponent<Rigidbody2D>().sharedMaterial;
            return;
        }
        ResetProperties();        
    }

    void Pushing() 
    {
        if (_player == null) 
        {
            ResetProperties();
            return;
        }
        _rb.velocity = new Vector2(_player.Velocity.x, _rb.velocity.y);     
        _player.PlayerState = PlayerStates.Pushing;
    }

    void ResetProperties() 
    {
        gameObject.layer = _onTop ? _groundLayerMask : _hideableLayerMask;
        _rb.sharedMaterial = null;
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _pushing = false;
        _player.PlayerState = _player.PlayerState == PlayerStates.Pushing ? _player.PlayerState = PlayerStates.Default : _player.PlayerState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * _jumpLowerOffset, new Vector2(1, 1f));
        Gizmos.DrawWireCube(transform.position + Vector3.up * _jumpHigherOffset, new Vector2(1, 1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + ((Vector3)(-_pushingDirection) * _pushOffset), new Vector2(1.1f, 0.8f));
        Gizmos.DrawWireCube(transform.position + ((Vector3)(-_pushingDirection) * 0.01f), new Vector2(0.8f, 0.8f));
        Gizmos.color = Color.white;
    }
}

