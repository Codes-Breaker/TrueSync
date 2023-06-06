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
    /// �ض�����µ��ߴӽ�ɫ�������Ƴ��������ڵ������ɵ����
    /// </summary>
    public virtual void LossItemAbility()
    {

    }

    public virtual void End()
    {
        character.RemoveItemAbility();
    }
}
