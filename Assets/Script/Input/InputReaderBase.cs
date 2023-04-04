using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine;

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
    [HideInInspector]
    public CharacterContorl player;



    public bool hasJumped = false;
    public bool skippedFrame = false;
    public bool isMouseAndKeyboard = true;
    public bool oldInput = true;
}
