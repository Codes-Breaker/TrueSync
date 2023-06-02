using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum ItemType
{
    Trap,
    Projectile,
    Buff
}

[Serializable]
public struct ItemData
{
    public int itemId;
    public ItemType itemType;
    public int canUseNum;
    public float leftTime;
    public GameObject itemPrefab;
}

public class ItemBase : MonoBehaviour
{
    //private float stayTime;
    //private float currentTime;
    //private bool isShow = false;
    [SerializeField]
    public ItemData ItemData;
    private GameObject itemGameObject;


    public ItemBase(ItemData data)
    {
        ItemData = data;
    }
    //public ItemBuffBase itemBuffBase;

    public void Init()
    {
        CreatItemGameObject();
    }

    public void CreatItemGameObject()
    {
        itemGameObject = Instantiate<GameObject>(ItemData.itemPrefab,this.transform);
        itemGameObject.SetActive(true);
        itemGameObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public ItemAbilityBase CreatItemAbility(ItemData itemData)
    {
        return null;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();

            //var itemBuff = CreatItemBuff(collision.gameObject.GetComponent<CharacterContorl>());
            //otherCollision.buffs.Add(itemBuff);
        }
    }

    //protected virtual ItemBuffBase CreatItemBuff(CharacterContorl target)
    //{
    //    return null;
    //}

    // Update is called once per frame
    private void Update()
    {
        //if (!isShow)
        //    return;
        //currentTime += Time.deltaTime;
        //if (currentTime > stayTime)
        //{
        //    OnEnd();
        //}

    }

    public virtual void OnEnd()
    {
        DOTween.Kill(itemGameObject.transform);
        GameObject.Destroy(gameObject);
    }
}
