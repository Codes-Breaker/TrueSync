using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAbilityBase 
{
    public CharacterContorl character;
    public GameObject itemGameObject;
    ItemData data;
    public ItemAbilityBase(CharacterContorl character,ItemData data)
    {
        this.data = data;
        this.character = character;
    }

    public virtual void Init()
    {

    }

    public virtual void Update()
    {

    }

    public virtual void FixedUpdate()
    {

    }

    protected virtual void UseItemAbility()
    {

    }

    protected virtual void OnItemReduced()
    {

    }


    protected virtual void CloseItemAbility()
    {

    }

    /// <summary>
    /// 特定情况下道具从角色的身上移除，重新在地上生成的情况
    /// </summary>
    public virtual void LossItemAbility()
    {

    }

    public virtual void End()
    {
        character.RemoveItemAbility();
    }
}
