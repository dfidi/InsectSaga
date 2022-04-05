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
    protected Vector2 _normalSurface;

    protected void ChangeTryToRunState() => _tryToRun = !_tryToRun;

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

    private RaycastHit2D hit1, hit2;
    private void Move()
    {
        if (MoveDir < 0) { _flip = true; }
        else if (MoveDir > 0) { _flip = false; }

        spriteRenderer.flipX = _flip;
        RunDir = _flip ? -1 : 1;

        if (currentState == PlayerState.Normal)
            _rb2d.velocity = new Vector2(moveSpeed * MoveDir, _rb2d.velocity.y);
        else if (currentState == PlayerState.Run)
        {
            hit1 = Physics2D.Raycast(transform.position, transform.right*RunDir, _distanceToWall, groundLayer);
            hit2 = Physics2D.Raycast(transform.position + transform.right*RunDir*0.5f, -transform.up, 0.5f, groundLayer);

            if (hit2.collider == null)
            {
                if (_flip)
                {
                    transform.Rotate(0,0,rotationSpeed*Time.fixedDeltaTime);
                }
                else
                {
                    transform.Rotate(0,0,-rotationSpeed*Time.fixedDeltaTime);
                }
            }
            else if (hit1.collider != null)
            {
                if (_flip)
                {
                    transform.Rotate(0,0,-rotationSpeed*Time.fixedDeltaTime);
                }
                else
                {
                    transform.Rotate(0,0,rotationSpeed*Time.fixedDeltaTime);
                }
            }

            Vector2 pos = _rb2d.position + (Vector2) transform.right * RunDir * runSpeed * Time.fixedDeltaTime;
            pos -= (Vector2) transform.up * Time.fixedDeltaTime*2;
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
    
    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //         _normalSurface = collision.contacts[0].normal;
    // }
}
