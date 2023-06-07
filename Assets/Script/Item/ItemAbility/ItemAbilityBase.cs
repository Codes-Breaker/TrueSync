using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAbilityBase 
{
    public CharacterContorl character;
    public GameObject itemGameObject;
    protected ItemData itemData;
    public ItemAbilityBase(CharacterContorl character,ItemData data)
    {
        this.itemData = data;
        this.character = character;
    }

    public virtual void Init()
    {
        EquipCharacter();
    }

    protected virtual void EquipCharacter()
    {
        itemGameObject = Object.Instantiate<GameObject>(itemData.itemPrefabOnCharacter,character.itemPlace);
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
        if(character.isDead || character.isStun)
        {
            LossItemAbility();
        }
    }

    public virtual void UseItemAbility()
    {
        OnItemReduced();
        if (!CheckQuantityOfItem())
        {
            End();
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
        character = null;
    }
}
