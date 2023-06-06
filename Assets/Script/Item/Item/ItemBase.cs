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

    /// <summary>
    /// ����λ�õ�
    /// </summary>
    /// <param name="data"></param>
    public ItemBase(ItemData data)
    {
        itemData = data;
        Init();
    }

   /// <summary>
   /// ��λ�õ�
   /// </summary>
    public ItemBase(ItemData data,Vector3 point)
    {
        itemData = data;
        Init(point);
    }
    //public ItemBuffBase itemBuffBase;

    public void Init()
    {
        CreatItemGameObject();
        SetColliderTrigger();
    }

    public void Init(Vector3 point)
    {
        CreatItemGameObject(point);
        SetColliderTrigger();
    }

    private void SetColliderTrigger()
    {
        GetComponent<Collider>().isTrigger = true;
    }


    /// <summary>
    /// ��������λ�õ�
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
    /// ������λ�õ�
    /// </summary>
    public void CreatItemGameObject(Vector3 point)
    {
        itemGameObject = Instantiate<GameObject>(itemData.itemPrefabOnGround, this.transform);
        itemGameObject.SetActive(true);
        itemGameObject.transform.position = point;
        itemGameObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider collision)
    {
        var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
        if (otherCollision)
        {
            if(otherCollision.itemAbility != null)
            {
                var itemAbility =  ItemManager.CreatItemAbilityByItemData(itemData, otherCollision);
                otherCollision.GainItemAbility(itemAbility);
                OnEnd();
            }
        }
    }

 

    public virtual void OnEnd()
    {
        DOTween.Kill(itemGameObject.transform);
        GameObject.Destroy(gameObject);
    }
}
