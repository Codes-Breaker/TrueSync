using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public Action onRocketCallback;
    public Action onBoomThrowCallback;
    public Action onBananaTakeCallback;
    public Action onBananaEatCallback;
    public Action onBananaThrowCallback;
    public Action onShoryukenEnd;
    public Action onShoryukenStart;

    public enum EventEnum
    {
        OnRocket,
        ThrowBoom,
        BananaTake,
        BananaEat,
        BananaThrow,
        ShoryukenEnd,
        ShoryukenStart,
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
            case EventEnum.BananaTake:
                onBananaTakeCallback?.Invoke();
                break;
            case EventEnum.BananaEat:
                onBananaEatCallback?.Invoke();
                break;
            case EventEnum.BananaThrow:
                onBananaThrowCallback?.Invoke();
                break;
            case EventEnum.ShoryukenEnd:
                onShoryukenEnd?.Invoke();
                break;
            case EventEnum.ShoryukenStart:
                onShoryukenStart?.Invoke();
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
            case EventEnum.BananaTake:
                onBananaTakeCallback += callback;
                break;
            case EventEnum.BananaEat:
                onBananaEatCallback += callback;
                break;
            case EventEnum.BananaThrow:
                onBananaThrowCallback += callback;
                break;
            case EventEnum.ShoryukenEnd:
                onShoryukenEnd += callback;
                break;
            case EventEnum.ShoryukenStart:
                onShoryukenStart += callback;
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
            case EventEnum.BananaTake:
                onBananaTakeCallback -= callback;
                break;
            case EventEnum.BananaEat:
                onBananaEatCallback -= callback;
                break;
            case EventEnum.BananaThrow:
                onBananaThrowCallback -= callback;
                break;
            case EventEnum.ShoryukenEnd:
                onShoryukenEnd -= callback;
                break;
            case EventEnum.ShoryukenStart:
                onShoryukenStart -= callback;
                break;
            default:
                break;
        }
    }

}
