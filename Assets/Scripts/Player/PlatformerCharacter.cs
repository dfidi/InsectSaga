using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Параметры движения")]
    public float moveSpeed;
    public float runSpeed;
    public float jumpPower;
    public float rotationSpeed;

    [Header("Другие параметры")]
    public LayerMask groundLayer;
    public float groundCheckRadius;

    [Header("Компоненты")]
    public Transform collTransform;
    public Transform groundChecker;
    public SpriteRenderer spriteRenderer;
    public Sprite walkSprite;
    public Sprite runSprite;
    
    public enum PlayerState { Normal, Run}
    
    [Header("Вспомогательное")]
    public PlayerState currentState;
    
    //VARIABLES
    private Rigidbody2D _rb2d;
    private bool _flip;
    private bool _tryToRun;
    private float _distanceToWall;
    protected float MoveDir;
    protected int RunDir = 1;

    protected void ChangeTryToRunState() => _tryToRun = true;

    protected void ChangeToRunState()
    {
        if (currentState == PlayerState.Run) return;
        if (!CheckGround()) return;
        var r = Quaternion.Euler(new Vector3(0, 0, -90));
        spriteRenderer.sprite = runSprite;
        collTransform.localRotation = r;
        _rb2d.constraints = RigidbodyConstraints2D.None;
        _rb2d.gravityScale = 0;
        currentState = PlayerState.Run;
    }
    
    protected void ReturnToNormalState()
    {
        if (currentState == PlayerState.Normal) return;
        Debug.Log("Return To normal state");
        _tryToRun = false;
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb2d.gravityScale = 7;
        spriteRenderer.sprite = walkSprite;
        collTransform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        currentState = PlayerState.Normal;
    }

    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);
    }
    
    protected void Jump()
    {
        if (!CheckGround() || currentState == PlayerState.Run) return;
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpPower);
    }

    private RaycastHit2D rayGround, rayWall;
    private void Move()
    {
        if (MoveDir < 0)
        {
            _flip = true;
            RunDir = 1;
        }
        else if (MoveDir > 0)
        {
            _flip = false;
            RunDir = -1;
        }

        spriteRenderer.flipX = _flip;

        if (currentState == PlayerState.Normal)
            _rb2d.velocity = new Vector2(moveSpeed * MoveDir, _rb2d.velocity.y);
        else if (currentState == PlayerState.Run)
        {
            var transformUp = transform.up;
            var transformRight = transform.right * RunDir;
            
            rayGround = Physics2D.Raycast(transform.position, transformRight, _distanceToWall, groundLayer);
            rayWall = Physics2D.Raycast(transform.position + transformRight*0.5f, -transformUp, 0.5f, groundLayer);

            if (rayWall.collider is null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
            }
            else if (rayGround.collider != null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
            }

            Vector2 pos = _rb2d.position + (Vector2) transformRight * runSpeed * Time.fixedDeltaTime;
            pos -= (Vector2) transformUp * Time.fixedDeltaTime*2;
            _rb2d.MovePosition(pos);
        }
    }

    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Normal;
        _distanceToWall  = 0.5f+1 / Mathf.Sqrt(2);
    }
    
    
    protected virtual void Update()
    {
        if (_tryToRun) ChangeToRunState();
        //Debug.DrawRay(transform.position, transform.right*2, Color.red);
        if (currentState == PlayerState.Run)
        {
            Debug.DrawRay(transform.position, transform.right * RunDir * _distanceToWall, Color.red);
            Debug.DrawRay(transform.position + transform.right * RunDir * 0.5f, -transform.up * 0.3f, Color.red);
        }
    }

    private void FixedUpdate()
    {
        Move();

    }
}
