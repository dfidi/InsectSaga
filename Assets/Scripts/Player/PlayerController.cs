using System;

public class PlayerController : InsectController
{
    private InputScheme _input;
    
    private void AttachInput()
    {
        _input = new InputScheme();
        
        _input.Player.Jump.performed += context => { Jump(); };
        _input.Player.RunMode.performed += context => { WalkRunMode(); };
    }

    void Awake()
    {
        AttachInput();
    }

    private void OnEnable()
    {
        _input.Enable();
        
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        float dir = _input.Player.Move.ReadValue<float>();
        Move(dir);
    }
}
