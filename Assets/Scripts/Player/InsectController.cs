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

    public float minDistanceToWall = 0.5f;
    
    protected bool Grounded;
    protected bool IsFlipped;
    
    public enum InsectState { Walk, Run }

    public InsectState curState;
    
    private Rigidbody2D _rb2d;

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

    protected void TryEnableRunMode()
    {
        if (!CheckWall()) return;
        Debug.Log("WalkRun Mode Changed!");
        curState = InsectState.Run;
    }

    protected void DisableRunMode()
    {
        curState = InsectState.Walk;
    }
    
    protected void WalkRunMode()
    {
        if (!CheckWall()) return;
        Debug.Log("WalkRun Mode Changed!");
        curState = curState == InsectState.Run ? InsectState.Walk : InsectState.Run;
    }

    protected void Jump()
    {
        if (!Grounded) return;
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, curState == InsectState.Run ? jumpPower : jumpPowerWalk);
    }

    protected virtual void Start()
    {
        curState = InsectState.Walk;
        _rb2d = GetComponent<Rigidbody2D>();
    }


    protected virtual void Update()
    {
        Debug.DrawRay(transform.position, Vector2.right * (IsFlipped ? -1 : 1)*minDistanceToWall, Color.red);
    }

    protected void Move(float dir)
    {
        if (dir < -0.01f) IsFlipped = true;
        else if (dir > 0.01f) IsFlipped = false;
        
        if (curState == InsectState.Run)
        {
            _rb2d.velocity = new Vector2(moveSpeed*dir, _rb2d.velocity.y);
        }
        else
        {
            _rb2d.velocity = new Vector2(moveSpeedWalk*dir, _rb2d.velocity.y);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!Grounded && other.CompareTag("Ground")) Grounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground")) Grounded = false;
    }
}
