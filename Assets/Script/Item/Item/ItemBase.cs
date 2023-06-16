using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


[RequireComponent(typeof(BoxCollider))]
public class ItemBase : MonoBehaviour
{
    //private float stayTime;
    //private float currentTime;
    //private bool isShow = false;
    public ItemData itemData;

    private GameObject itemGameObject;
    private float lastTime;
    private float maxTime = 30;
    private void Start()
    {
        lastTime = 0;
        Init();
    }

    public void Init()
    { 
        CreatItemGameObject();
        SetColliderTrigger();
    }


    public void Init(ItemData data)
    {
        itemData = data;
        CreatItemGameObject();
        SetColliderTrigger();
    }

    public void Init(ItemData data,Vector3 point)
    {
        itemData = data;
        CreatItemGameObject(point);
        SetColliderTrigger();
    }

    private void SetColliderTrigger()
    {
        GetComponent<Collider>().isTrigger = true;
    }


    /// <summary>
    /// 不带出生位置的
    /// </summary>
    public void CreatItemGameObject()
    {
        itemGameObject = Instantiate<GameObject>(itemData.itemPrefabOnGround,this.transform);
        itemGameObject.SetActive(true);
        itemGameObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    /// <summary>
    /// 带出生位置的
    /// </summary>
    public void CreatItemGameObject(Vector3 point)
    {
        itemGameObject = Instantiate<GameObject>(itemData.itemPrefabOnGround, this.transform);
        itemGameObject.SetActive(true);
        transform.position = point;
        itemGameObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider collision)
    {
        var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
        if (otherCollision && !otherCollision.isStun)
        {
            if(otherCollision.itemAbility == null && otherCollision.CanPick())
            {
                var itemAbility =  ItemManager.CreatItemAbilityByItemData(itemData, otherCollision);
                otherCollision.GainItemAbility(itemAbility);
                OnEnd();
            }
        }
    }

    private void FixedUpdate()
    {
        lastTime += Time.fixedDeltaTime;
        if (lastTime > maxTime && maxTime != -1)
        {
            this.OnEnd();
        }
    }



    public virtual void OnEnd()
    {
        DOTween.Kill(itemGameObject.transform);
        GameObject.Destroy(gameObject);
    }
}
