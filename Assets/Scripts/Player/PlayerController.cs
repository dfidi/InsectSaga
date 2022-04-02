using System;

public class PlayerController : InsectController
{
    private InputScheme _input;

    private void AttachInput()
    {
        _input = new InputScheme();

        //_input.Player.Move.ReadValue<float>();
        _input.Player.Jump.performed += context => { };
        _input.Player.RunMode.performed += context => { };
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
        
    }

    protected override void Update()
    {
        
    }
}
