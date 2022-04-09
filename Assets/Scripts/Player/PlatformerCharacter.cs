using System.Collections;
using DG.Tweening;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlatformerCharacter : MonoBehaviour
{
    [Header("Параметры движения")]
    public float moveSpeed;
    public float crawlSpeed;
    public float jumpPower;
    public float rotationSpeed;
    [Tooltip("Минимальная дистанция прицепления к стене")] public float grabWallDistance;
    [Tooltip("Минимальная дистанция приципления к полу/потолку")] public float grabGroundDistance;

    [Header("Другие параметры")]
    public LayerMask groundLayer;
    public float groundCheckRadius;
    public float rayGroundLength;

    [Header("Компоненты")]
    public Animator animator;
    public Transform collTransform;
    public Transform groundChecker;
    public SpriteRenderer spriteRenderer;

    public enum PlayerState { Normal, Crawl}
    
    [Header("Вспомогательное")]
    public PlayerState currentState;
    
    //VARIABLES
    private Rigidbody2D _rb2d;
    private bool _flip;
    private bool _isJump;
    protected bool TryToRun;
    private float _distanceToWall;
    protected float MoveDir;
    protected int CrawlDir = 1;

    protected void ToRunState()
    {
        if (currentState == PlayerState.Crawl) return;

        if (!CheckRunGround() && !CheckRunWall()) return;

        Quaternion angleRot = Quaternion.Euler(0, 0, 0);

        if (CheckRunWall())
           angleRot = Quaternion.Euler(0, 0, 90*CrawlDir);
        
        collTransform.localRotation = Quaternion.Euler(0, 0, -90);
        transform.rotation = angleRot;
        
        animator.SetBool("is_crawl", true);
        _rb2d.constraints = RigidbodyConstraints2D.None;
        _rb2d.gravityScale = 0;
        currentState = PlayerState.Crawl;
    }
    
    protected void ToNormalState()
    {
        TryToRun = false;
        if (currentState == PlayerState.Normal) return;
        animator.SetBool("is_crawl", false);

        collTransform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _rb2d.velocity = Vector2.zero; //Reset velocity, otherwise accumulates
        _rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        _rb2d.gravityScale = 7;
        currentState = PlayerState.Normal;
    }
    
    protected void Jump()
    {
        if (!CheckGround() || currentState == PlayerState.Crawl) return;
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, jumpPower);
        _isJump = true;
    }

    #region RAYCASTS
    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, groundLayer);
    }
    
    private RaycastHit2D CheckRunGround()
    {
        return Physics2D.Raycast(transform.position, -transform.up, grabGroundDistance, groundLayer);
    }

    private RaycastHit2D CheckRunWall()
    {
        return Physics2D.Raycast(transform.position, transform.right*CrawlDir, grabWallDistance, groundLayer);
    }
    #endregion

    private RaycastHit2D rayGround, rayWall;
    private void Move()
    {
        if (currentState == PlayerState.Normal)
            _rb2d.velocity = new Vector2(moveSpeed * MoveDir, _rb2d.velocity.y);
        else if (currentState == PlayerState.Crawl)
        {
            var transformUp = transform.up;
            var transformRight = transform.right * CrawlDir;
            
            rayGround = Physics2D.Raycast(transform.position, transformRight, _distanceToWall, groundLayer);
            rayWall = Physics2D.Raycast(transform.position + transformRight*0.5f, -transformUp, rayGroundLength, groundLayer);

            if (rayWall.collider is null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? rotationSpeed : -rotationSpeed));
            }
            else if (rayGround.collider != null)
            {
                _rb2d.MoveRotation(_rb2d.rotation + (_flip ? -rotationSpeed : rotationSpeed));
            }

            Vector2 pos = _rb2d.position + (Vector2) (transformRight * crawlSpeed * Time.fixedDeltaTime - transformUp * Time.fixedDeltaTime*2);
            _rb2d.MovePosition(pos);
        }
    }

    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        currentState = PlayerState.Normal;
        _distanceToWall  = 0.5f+1 / Mathf.Sqrt(2);
    }

    private void UpdatePlayerDirection()
    {
        if (MoveDir < 0)
        {
            _flip = true;
            CrawlDir = -1;
        }
        else if (MoveDir > 0)
        {
            _flip = false;
            CrawlDir = 1;
        }

        spriteRenderer.flipX = _flip;
    }


    private void DrawDebugRays()
    {
        if (currentState == PlayerState.Crawl)
        {
            Debug.DrawRay(transform.position, transform.right * CrawlDir * _distanceToWall, Color.red);
            Debug.DrawRay(transform.position + transform.right * CrawlDir * 0.5f, -transform.up * rayGroundLength, Color.red);
        }
        Debug.DrawRay(transform.position, transform.right * CrawlDir * grabWallDistance, Color.blue);
        Debug.DrawRay(transform.position, -transform.up * grabGroundDistance, Color.blue);
    }
    
    protected virtual void Update()
    {
        if (TryToRun) ToRunState();
        UpdatePlayerDirection();
        DrawDebugRays();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
