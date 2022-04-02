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
    
    protected bool Grounded;
    
    protected enum InsectState
    {
        Walk,
        Run
    }

    protected InsectState CurState = InsectState.Walk;

    
    private Rigidbody2D _rb2d;
    
    protected void WalkRunMode()
    {
        CurState = CurState == InsectState.Run ? InsectState.Walk : InsectState.Run;
    }

    protected void Jump()
    {
        if (!Grounded) return;
        _rb2d.velocity = new Vector2(_rb2d.velocity.x, CurState == InsectState.Run ? jumpPower : jumpPowerWalk);
    }

    protected virtual void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }


    protected virtual void Update()
    {
        
    }

    protected void Move(float dir)
    {
        if (CurState == InsectState.Run)
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
