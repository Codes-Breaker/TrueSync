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
    public float leftTime;
    public GameObject itemPrefabOnGround;
    public GameObject itemPrefabOnCharacter;
}

public class ItemManager
{
    public static ItemAbilityBase CreatItemAbilityByItemData(ItemData itemData,CharacterContorl character)
    {
        switch(itemData.itemId)
        {
            case 1:
                return new EnlargementPotionAbility(character, itemData);
            case 2:
                return new RocketThrusterAbility(character, itemData);
            default:
                Debug.LogError("没有找到对应道具ID");
                return null;
        }    
    }
}
