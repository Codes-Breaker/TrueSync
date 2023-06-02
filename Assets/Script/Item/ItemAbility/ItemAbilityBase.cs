using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAbilityBase : MonoBehaviour
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



    protected virtual void UseItemAbility()
    {

    }

    protected virtual void OnItemReduced()
    {

    }


    protected virtual void CloseItemAbility()
    {

    }

    public virtual void End()
    {

    }
}
