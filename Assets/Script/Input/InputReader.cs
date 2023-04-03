using UnityEngine;
using UnityEngine.Events;

//DISABLE if using old input system
using UnityEngine.InputSystem;


public class InputReader : InputReaderBase
{
        

    //DISABLE if using old input system
    private PlayerOneMovementActions movementActions;


    /**/


    //DISABLE if using old input system
    private void Awake()
    {
        movementActions = new PlayerOneMovementActions();

        movementActions.Gameplay.Movement.performed += ctx => OnMove(ctx);

        movementActions.Gameplay.Jump.performed += ctx => OnJump();
        movementActions.Gameplay.Jump.canceled += ctx => JumpEnded();

        movementActions.Gameplay.Camera.performed += ctx => OnCamera(ctx);

        movementActions.Gameplay.Charge.performed += ctx => OnCharge(ctx);
        movementActions.Gameplay.Charge.canceled += ctx => ChargeEnded(ctx);

        movementActions.Gameplay.Pull.performed += ctx => OnPull(ctx);
        movementActions.Gameplay.Pull.canceled += ctx => PullEnd(ctx);

        movementActions.Gameplay.InteractWeapon.performed += ctx => OnInteract(ctx);
        movementActions.Gameplay.InteractWeapon.canceled += ctx => InteractEnd(ctx);
    }


    //ENABLE if using old input system
    private void Update()
    {
        /*
             
        axisInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;

        if (enableJump)
        {
            if (Input.GetButtonDown("Jump")) OnJump();
            if (Input.GetButtonUp("Jump")) JumpEnded();
        }

        if (enableSprint) sprint = Input.GetButton("Fire3");
        if (enableCrouch) crouch = Input.GetButton("Fire1");

        GetDeviceOld();

        */
    }


    //DISABLE if using old input system
    private void GetDeviceNew(InputAction.CallbackContext ctx)
    {
        oldInput = isMouseAndKeyboard;

        if (ctx.control.device is Keyboard || ctx.control.device is Mouse) isMouseAndKeyboard = true;
        else isMouseAndKeyboard = false;

        if (oldInput != isMouseAndKeyboard && isMouseAndKeyboard) changedInputToMouseAndKeyboard.Invoke();
        else if (oldInput != isMouseAndKeyboard && !isMouseAndKeyboard) changedInputToGamepad.Invoke();
    }


    //ENABLE if using old input system
    private void GetDeviceOld()
    {
        /*

        oldInput = isMouseAndKeyboard;

        if (Input.GetJoystickNames().Length > 0) isMouseAndKeyboard = false;
        else isMouseAndKeyboard = true;

        if (oldInput != isMouseAndKeyboard && isMouseAndKeyboard) changedInputToMouseAndKeyboard.Invoke();
        else if (oldInput != isMouseAndKeyboard && !isMouseAndKeyboard) changedInputToGamepad.Invoke();

        */
    }


    #region Actions

    //DISABLE if using old input system
    public void OnMove(InputAction.CallbackContext ctx)
    {
        axisInput = ctx.ReadValue<Vector2>();
        Debug.LogError($"axisinput: {axisInput}");
        GetDeviceNew(ctx);
    }


    public void OnJump()
    {
        if (enableJump)
        {
            jump = true;
            jumpHold = true;

            hasJumped = true;
            skippedFrame = false;
        }
    }


    public void JumpEnded()
    {
        jump = false;
        jumpHold = false;
    }



    private void FixedUpdate()
    {
        if (hasJumped && skippedFrame)
        {
            jump = false;
            hasJumped = false;
        }
        if (!skippedFrame && enableJump) skippedFrame = true;
    }



    //DISABLE if using old input system
    public void OnCamera(InputAction.CallbackContext ctx)
    {
        float pointerDelta = ctx.ReadValue<float>();
        cameraInput.x = pointerDelta;
        GetDeviceNew(ctx);
    }


    //DISABLE if using old input system
    public void OnCharge(InputAction.CallbackContext ctx)
    {
        if (enableSprint) charge = true;
    }


    //DISABLE if using old input system
    public void ChargeEnded(InputAction.CallbackContext ctx)
    {
        charge = false;
    }

    public void OnPull(InputAction.CallbackContext ctx)
    {
        pull = true;
    }

    public void PullEnd(InputAction.CallbackContext ctx)
    {
        pull = false;
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        interact = true;
    }

    public void InteractEnd(InputAction.CallbackContext ctx)
    {
        interact = false;
    }

    #endregion


    #region Enable / Disable

    //DISABLE if using old input system
    private void OnEnable()
    {
        movementActions.Enable();
    }


    //DISABLE if using old input system
    private void OnDisable()
    {
        movementActions.Disable();
    }

    #endregion
}
