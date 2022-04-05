using System;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public class InsectController : MonoBehaviour
{
    [Header("Стоячий режим")]
    public float moveSpeedWalk = 1;
    public float jumpPowerWalk = 1;
    
    [Header("Ползающий режим")]
    public float moveSpeed = 1;
    public float jumpPower = 1;

    [Space]

    public float minDistanceToWall = 0.5f;

    [Space]
    public GameObject walkBody;
    public GameObject runBody;

    public Transform checkGroundCircle;
    public LayerMask groundLayer;

    public SpriteRenderer walkSprite;
    public SpriteRenderer runSprite;

    protected float MoveDir;
    
    protected bool IsVerticalInput;
    protected bool Grounded;
    protected bool IsFlipped;
    
    public enum InsectState { Walk, Run }

    public InsectState curState;

    public InsectLine moveLine;
    
    private Rigidbody2D _rb2d;
    private Vector2 _normalSurface;
    private bool _isTryEnableRunMode;

    private int _currentPoint;
    
    protected void ChangeStateEnableRunMode()
    {
        _isTryEnableRunMode = !_isTryEnableRunMode;
    }

    private bool CheckWall()
    {
        return Physics2D.Raycast(transform.position, Vector2.right * (IsFlipped ? -1 : 1), minDistanceToWall,
            groundLayer);
    }

    protected void EnableRunMode()
    {
        if (curState == InsectState.Run) return;
        if (!CheckWall()) return;
        _rb2d.gravityScale = 0;
        Debug.Log("WalkRun Mode Changed!");
        curState = InsectState.Run;
        walkBody.SetActive(false);
        runBody.SetActive(true);
        
        _rb2d.velocity = Vector2.zero;

        var p1 = moveLine[_currentPoint].position;
        var p2 = moveLine[_currentPoint+1].position;
        var p = _rb2d.position;
        
        //Calculate start position when stick to surface
        t = Vector2.Distance(p1, p)/Vector2.Distance(p1,p2);

        p = (p2 - p1).normalized;
        IsVerticalInput = Mathf.Abs(p.y) > Mathf.Abs(p.x);
        runSprite.flipY = IsFlipped;
    }

    public void IntersectWithPath(InsectLine moveLine, int index)
    {
        if (!_isTryEnableRunMode) return;
        _currentPoint = index;
        this.moveLine = moveLine;
        EnableRunMode();
    }
    

    protected void DisableRunMode()
    {
        if (curState == InsectState.Walk) return;
        _rb2d.gravityScale = 7;
        curState = InsectState.Walk;
        walkBody.SetActive(true);
        runBody.SetActive(false);
        IsVerticalInput = false;
        moveLine = null;
    }

    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(checkGroundCircle.position, 0.15f, groundLayer);
    }

    protected void Jump()
    {
        if (!CheckGround()) return;
        if (curState == InsectState.Run) return;
        _rb2d.AddForce(Vector2.up*jumpPower, ForceMode2D.Impulse);
    }
    
    protected virtual void Start()
    {
        curState = InsectState.Walk;
        _rb2d = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        walkSprite.flipY = IsFlipped;
        if (curState == InsectState.Run)
        {
            runSprite.flipX = IsFlipped;
        }
        //if (_isTryEnableRunMode) TryEnableRunMode();

        Debug.DrawRay(transform.position, Vector2.right * (IsFlipped ? -1 : 1) * minDistanceToWall, Color.green);
        //Debug.DrawRay(transform.position, _normalSurface*1.5f, Color.green);
        //Debug.DrawRay(transform.position + new Vector3(0.3f, 0.4f, 0), Vector2.right*-0.5f, Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        // if (checkGroundCircle != null)
        //     Gizmos.DrawWireSphere(checkGroundCircle.position, 0.2f);
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    private int GetOrient()
    {
        if (_normalSurface.x!= 0)
            return _normalSurface.x > 0 ? 1 : -1;
        return _normalSurface.y > 0 ? -1 : 1;
    }

    private float t = 0.5f;

    private void Move()
    {
        if (curState == InsectState.Run)
        {
            t = Mathf.Clamp01(t+MoveDir*moveSpeed*Time.fixedDeltaTime/Vector2.Distance(moveLine[_currentPoint].position, moveLine[_currentPoint+1].position));
            _rb2d.MovePosition(Vector3.LerpUnclamped(moveLine[_currentPoint].position, moveLine[_currentPoint+1].position, t));
        }
        else
        {
            _rb2d.velocity = new Vector2(moveSpeedWalk*MoveDir, _rb2d.velocity.y);
        }
    }

    // private void OnCollisionStay2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Ground"))
    //         _normalSurface = collision.contacts[0].normal;
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!Grounded && other.CompareTag("Ground")) Grounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }
}
