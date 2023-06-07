using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject itemPrefabOnGround;
    public GameObject itemPrefabOnCharacter;
    public float CountDownTime;
}

public class ItemManager
{
    public static ItemAbilityBase CreatItemAbilityByItemData(ItemData itemData,CharacterContorl character)
    {
        switch(itemData.itemType)
        {
            case ItemType.Trap:
                return new TrapItemAbility(character, itemData);
            case ItemType.Projectile:
                return new ProjectileItemAbility(character, itemData);
            case ItemType.Buff:
                return new BuffItemAbility(character, itemData);
            default:
                return null;
        }    
    }

    public static ItemBuffBase CreatItemBuffByItemID(float ItemID,CharacterContorl character)
    {
        switch (ItemID)
        {
            case 1:
                return new RocketThrusterBuff(character);
            case 2:
                return new LargementPotionBuff(character);
            default:
                UnityEngine.Debug.LogError("û���ҵ���ӦID");
                return null;

        }


    }

}