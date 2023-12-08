using UnityEngine;
using UnityEngine.InputSystem;

public class HoverbikeInputProvider : InputProvider, HoverbikeActionMap.IGameplayActions
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool accelerateInput;
    public bool deaccelerateInput;

    protected override IInputActionCollection CreateMap() => new HoverbikeActionMap();

    protected override void SetupMap()
    {
        (inputMap as HoverbikeActionMap)?.Gameplay.SetCallbacks(this);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnAccelerate(InputAction.CallbackContext context)
    {
        accelerateInput = context.ReadValueAsButton();
    }

    public void OnDeaccelerate(InputAction.CallbackContext context)
    {
        deaccelerateInput = context.ReadValueAsButton();
    }
}