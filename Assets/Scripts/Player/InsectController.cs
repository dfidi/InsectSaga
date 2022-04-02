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

    private Rigidbody2D _rb2d;

    protected virtual void Start()
    {
        
    }


    protected virtual void Update()
    {
        
    }

    protected void Move(float dir)
    {
        
    }
}
