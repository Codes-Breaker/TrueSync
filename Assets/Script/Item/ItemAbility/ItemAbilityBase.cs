using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAbilityBase 
{
    public CharacterContorl character;
    public GameObject itemGameObject;
    protected ItemData itemData;
    protected bool canUseItem;
    protected float currentTime;
    public ItemAbilityBase(CharacterContorl character,ItemData data)
    {
        this.itemData = data;
        this.character = character;
    }

    public virtual void Init()
    {
        EquipCharacter();
        canUseItem = true;
    }

    protected virtual void EquipCharacter()
    {
        switch(itemData.equipPlace)
        {
            case EquipPlace.OnHead:
                itemGameObject = Object.Instantiate<GameObject>(itemData.itemPrefabOnCharacter,character.itemPlaceHead);
                break;
            case EquipPlace.OnHeadStatic:
                itemGameObject = Object.Instantiate<GameObject>(itemData.itemPrefabOnCharacter, character.itemPlaceHeadStatic);
                break;
            case EquipPlace.OnDorsal:
                itemGameObject = Object.Instantiate<GameObject>(itemData.itemPrefabOnCharacter,character.itemPlace);
                break;

        }

        itemGameObject.SetActive(true);

    }

    protected virtual void UnEquipCharacter()
    {
        Object.Destroy(itemGameObject);
    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {
        if(character.isDead || character.HasQTEStun())
        {
            LossItemAbility();
        }
        if(canUseItem == false)
        {
            currentTime += Time.deltaTime;
            if(currentTime > itemData.CountDownTime)
            {
                canUseItem = true;
            }
        }
    }

    public virtual void UseItemAbility()
    {
        if (canUseItem == true)
        {
            itemAbility();
        }
    }

    protected virtual void itemAbility()
    {
        OnItemReduced();
        if (!CheckQuantityOfItem())
        {
            End();
        }
        else
        {
            canUseItem = false;
            currentTime = 0f;
        }
    }

    protected virtual void OnItemReduced()
    {
        itemData.canUseNum -- ;
    }

    protected virtual bool CheckQuantityOfItem()
    {
        if (itemData.canUseNum > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 特定情况下道具从角色的身上移除，重新在地上生成的情况
    /// </summary>
    public virtual void LossItemAbility()
    {
        GameObject item = new GameObject("Item");
        var itemBase = item.AddComponent<ItemBase>();
        itemBase.Init(itemData, character.ridbody.transform.position);
        End();
    }

    public virtual void End()
    {
        //销毁装备物体
        UnEquipCharacter();
        //清楚
        character.RemoveItemAbility();
        //character = null;
    }
}
