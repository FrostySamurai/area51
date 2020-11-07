using UnityEngine;

public class HideableObject : MonoBehaviour
{
    [SerializeField, Range(0,1)] float _pushOffset = 0.2f;
    [SerializeField, Range(0, 2)] float _jumpLowerOffset = 0.5f;
    [SerializeField, Range(1, 5)] float _jumpHigherOffset = 2f;
    [SerializeField] bool _jumpAble = false;
    [SerializeField] bool _pushAble = false;
    [SerializeField] private LayerMasksEnum _hideableLayerMask;
    [SerializeField] private LayerMasksEnum _groundLayerMask;  
    private bool _pushing = false;
    private bool _onTop = false;
    private Vector2 _pushingDirection = Vector2.zero;
    private bool _downKeyPressed = false;
    private bool _interactionKeyPressed = false;
    private PlayerController _player;

    private Rigidbody2D _rb;

    private void Start()
    {
        AppData.InputController.OnDownKey += DownKeyPressed;
        AppData.InputController.OnDownKeyReleased += DownKeyReleased;
        AppData.InputController.OnInteraction += InteractionPressed;
        AppData.InputController.OnInteractionReleased += InteractionReleased;

        _rb = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void OnDestroy()
    {
        if (AppData.InputController != null)
        {
            AppData.InputController.OnDownKey -= DownKeyPressed;
            AppData.InputController.OnDownKeyReleased -= DownKeyReleased;
            AppData.InputController.OnInteraction -= InteractionPressed;
            AppData.InputController.OnInteractionReleased -= InteractionReleased;
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
    void InteractionPressed() => _interactionKeyPressed = true;
    void InteractionReleased() => _interactionKeyPressed = false;

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
                return (int)_hideableLayerMask;
            }
            return (int)_groundLayerMask;
        }
        _onTop = false;
        return _pushing ? (int)_groundLayerMask : (int)_hideableLayerMask;
    }

    void PushCollider() 
    {
        if(!_pushing)
            _pushingDirection = _player.Direction;
        RaycastHit2D firstHit = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0, -_pushingDirection, _pushOffset, 1 << 10);
        RaycastHit2D secondHit = Physics2D.BoxCast(transform.position, new Vector2(0.8f, 0.8f), 0, -_pushingDirection, 0.01f, 1 << 10);

        if (_interactionKeyPressed && _player.IsGrounded && firstHit.collider != null && secondHit.collider == null)
        {
            _pushing = true;
            Push();
        }

        if (!_interactionKeyPressed)
        {
            ResetProperties();
        }
    }

    void Push() 
    {
        _pushing = _interactionKeyPressed;
        if (_pushing)
        {
            gameObject.layer = (int)_groundLayerMask;
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
        if(_pushing == false)
            return;
        _pushing = false;
        gameObject.layer = _onTop ? (int)_groundLayerMask : (int)_hideableLayerMask;
        _rb.sharedMaterial = null;
        _rb.velocity = new Vector2(0, _rb.velocity.y);    
        _player.PlayerState = _player.PlayerState == PlayerStates.Pushing ? _player.PlayerState = PlayerStates.Default : _player.PlayerState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position + Vector3.up * _jumpLowerOffset, new Vector2(1, 1f));
        Gizmos.DrawWireCube(transform.position + Vector3.up * _jumpHigherOffset, new Vector2(1, 1f));
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + ((Vector3)(-_pushingDirection) * _pushOffset), new Vector2(1f, 0.8f));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + ((Vector3)(-_pushingDirection) * 0.01f), new Vector2(0.8f, 0.8f));
        Gizmos.color = Color.white;
    }
}

