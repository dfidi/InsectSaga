using UnityEngine;

public class InsectPlayer : PlatformerCharacter
{
    private InputScheme _input;

    private void Awake()
    {
        _input = new InputScheme();
        
        _input.Player.Jump.performed += context => { Jump(); };
        _input.Player.RunModeOn.performed += context => { TryToRun = true; };
        _input.Player.RunModeOff.performed += context => { ToNormalState(); };
    }
    
    private void OnEnable() { _input.Enable(); }
    
    private void OnDisable() { _input.Disable(); }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        Vector2 inputDirection = _input.Player.Movement.ReadValue<Vector2>();
        MoveDir = inputDirection.x;
        animator.SetFloat("move_dir", Mathf.Abs(MoveDir));
        base.Update();
    }
}
