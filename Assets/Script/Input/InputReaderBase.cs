using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using UnityEngine;

public enum ControlDeviceType
{
    Mouse = 1,
    Keyboard =2,
    Gamepad = 3
}

public class InputReaderBase : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Input specs")]
    public UnityEvent changedInputToMouseAndKeyboard;
    public UnityEvent changedInputToGamepad;

    [Header("Enable inputs")]
    public bool enableJump = true;
    public bool enableCrouch = true;
    public bool enableSprint = true;


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


        if (oldInput != isMouseAndKeyboard && isMouseAndKeyboard) changedInputToMouseAndKeyboard.Invoke();
        else if (oldInput != isMouseAndKeyboard && !isMouseAndKeyboard) changedInputToGamepad.Invoke();
    }
}
