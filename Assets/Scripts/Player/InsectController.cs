using UnityEngine;

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

    protected float MoveDir;
    
    protected bool Grounded;
    protected bool IsFlipped;
    
    public enum InsectState { Walk, Run }

    public InsectState curState;

    public InsectLine moveLine;
    
    private Rigidbody2D _rb2d;
    private Vector2 _normalSurface;
    private bool _isTryEnableRunMode;

    private int _currentPoint;

    protected bool CheckWall()
    {
        RaycastHit2D[] res = Physics2D.RaycastAll(transform.position, Vector2.right * (IsFlipped ? -1 : 1), minDistanceToWall);

        foreach (var hit in res)
        {
            if (hit.collider.CompareTag("Ground"))
            {
                return true;
            }
        }
        
        return false;
    }

    protected bool CheckAngleTransition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0.3f, 0.4f, 0), Vector2.right, 0.5f);

        return hit.collider is null;
    }

    protected void ChangeStateEnableRunMode()
    {
        _isTryEnableRunMode = !_isTryEnableRunMode;
    }

    protected void TryEnableRunMode()
    {
        if (curState == InsectState.Run) return;
        //if (!CheckWall()) return;
        _rb2d.gravityScale = 0;
        Debug.Log("WalkRun Mode Changed!");
        curState = InsectState.Run;
        walkBody.SetActive(false);
        runBody.SetActive(true);

        var p1 = moveLine[_currentPoint].position;
        var p2 = moveLine[_currentPoint+1].position;
        var p = _rb2d.position;
        
        t = Vector2.Distance(p1, p)/Vector2.Distance(p1,p2);
    }

    public void SetPathAndEnableRunMode(InsectLine moveLine, int index)
    {
        if (!_isTryEnableRunMode || this.moveLine == moveLine) return;
        _currentPoint = index;
        this.moveLine = moveLine;
        TryEnableRunMode();
    }
    

    protected void DisableRunMode()
    {
        if (curState == InsectState.Walk) return;
        _rb2d.gravityScale = 7;
        curState = InsectState.Walk;
        moveLine = null;
        walkBody.SetActive(true);
        runBody.SetActive(false);
    }

    protected void Jump()
    {
        if (!Grounded) return;
        _rb2d.AddForce(Vector2.up*jumpPower, ForceMode2D.Impulse);
        //_rb2d.velocity += new Vector2(_rb2d.velocity.x, curState == InsectState.Run ? jumpPower : jumpPowerWalk);
    }

    protected virtual void Start()
    {
        curState = InsectState.Walk;
        _rb2d = GetComponent<Rigidbody2D>();
    }


    protected virtual void Update()
    {
        //if (_isTryEnableRunMode) TryEnableRunMode();
        
        Debug.DrawRay(transform.position, Vector2.right * (IsFlipped ? -1 : 1)*minDistanceToWall, Color.green);
        Debug.DrawRay(transform.position, _normalSurface*1.5f, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0.3f, 0.4f, 0), Vector2.right*-0.5f, Color.red);
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            _normalSurface = collision.contacts[0].normal;
    }

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
