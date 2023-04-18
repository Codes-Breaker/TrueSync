using UnityEngine;
using UnityEngine.Events;

//DISABLE if using old input system
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum ControlDeviceType
{
    Mouse = 1,
    Keyboard = 2,
    Gamepad = 3
}

public class InputReaderBase : MonoBehaviour
{
    [Header("Input specs")]
    public UnityEvent changedInputToMouseAndKeyboard;
    public UnityEvent changedInputToGamepad;

    [Header("Enable inputs")]
    public bool enableJump = true;
    public bool enableCrouch = true;
    public bool enableSprint = true;

    public InputDevice device;


    [HideInInspector]
    public Vector2 axisInput;
    [HideInInspector]
    public Vector2 cameraInput = Vector2.zero;
    [HideInInspector]
    public bool jump;
    [HideInInspector]
    public bool jumpHold;
    [HideInInspector]
    public float zoom;
    [HideInInspector]
    public bool charge;
    [HideInInspector]
    public bool pull;
    [HideInInspector]
    public bool interact;


    public ControlDeviceType controlDeviceType;
    public bool hasJumped = false;
    public bool skippedFrame = false;
    public bool isMouseAndKeyboard = true;
    public bool oldInput = true;


    //DISABLE if using old input system
    private PlayerOneMovementActions movementActions;


    /**/

    public void Init(InputDevice inputDevice)
    {
        device = inputDevice;
        movementActions = new PlayerOneMovementActions();

        
       
        movementActions.Action.Movement.performed += ctx => OnMove(ctx);

        movementActions.Action.Charge.performed += ctx => OnCharge(ctx);
        movementActions.Action.Charge.canceled += ctx => ChargeEnded(ctx);



        movementActions.Action.InteractWeapon.performed += ctx => OnInteract(ctx);
        movementActions.Action.InteractWeapon.canceled += ctx => InteractEnd(ctx);
        movementActions.Action.Enable();
    }

    public void GetDeviceNew(InputAction.CallbackContext ctx)
    {
        oldInput = isMouseAndKeyboard;

        if (ctx.control.device is Keyboard || ctx.control.device is Mouse) isMouseAndKeyboard = true;
        else isMouseAndKeyboard = false;

        if (ctx.control.device is Keyboard)
            controlDeviceType = ControlDeviceType.Keyboard;
        else if (ctx.control.device is Mouse)
            controlDeviceType = ControlDeviceType.Mouse;
        else
            controlDeviceType = ControlDeviceType.Gamepad;


        if (oldInput != isMouseAndKeyboard && isMouseAndKeyboard) changedInputToMouseAndKeyboard?.Invoke();
        else if (oldInput != isMouseAndKeyboard && !isMouseAndKeyboard) changedInputToGamepad?.Invoke();
    }

    #region Actions

    //DISABLE if using old input system
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        axisInput = ctx.ReadValue<Vector2>();
       // Debug.Log(axisInput);
        GetDeviceNew(ctx);
    }

    //DISABLE if using old input system
    public void OnCamera(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        float pointerDelta = ctx.ReadValue<float>();
        cameraInput.x = pointerDelta;
        GetDeviceNew(ctx);
    }


    //DISABLE if using old input system
    public void OnCharge(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        var button = (ButtonControl)ctx.control;
        if (button.wasPressedThisFrame)
            charge = true;
        else if (button.wasReleasedThisFrame)
            charge = false;

    }


    //DISABLE if using old input system
    public void ChargeEnded(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        charge = false;
    }


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        var button = (ButtonControl)ctx.control;
        if (button.wasPressedThisFrame)
            interact = true;
        else if (button.wasReleasedThisFrame)
            interact = false;

    }

    public void InteractEnd(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device != device)
            return;
        interact = false;
    }

    #endregion


    #region Enable / Disable


    //DISABLE if using old input system
    private void OnDisable()
    {
        movementActions.Disable();
    }

    #endregion
}
