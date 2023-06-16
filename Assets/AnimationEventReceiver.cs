using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public Action onRocketCallback;
    public Action onBoomThrowCallback;
    public enum EventEnum
    {
        OnRocket,
        ThrowBoom,
    }
    public void TriggerEvent(EventEnum e)
    {
        switch (e)
        {
            case EventEnum.OnRocket:
                onRocketCallback?.Invoke();
                break;
            case EventEnum.ThrowBoom:
                onBoomThrowCallback?.Invoke();
                break;
            default:
                break;
        }
    }

    public void RegisterEvent(EventEnum e, Action callback)
    {
        switch (e)
        {
            case EventEnum.OnRocket:
                onRocketCallback += callback;
                break;
            case EventEnum.ThrowBoom:
                onBoomThrowCallback += callback;
                break;
            default:
                break;
        }
    }

    public void UnRegisterEvent(EventEnum e, Action callback)
    {
        switch (e)
        {
            case EventEnum.OnRocket:
                onRocketCallback -= callback;
                break;
            case EventEnum.ThrowBoom:
                onBoomThrowCallback -= callback;
                break;
            default:
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
