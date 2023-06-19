using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

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



public class ItemManager : MonoBehaviour
{
    public List<GameObject> itemList;
    [Header("道具创建频率")]
    public float itemCreateFrequency;
    [Header("道具刷新点位父级")]
    public GameObject itemPosition;
    public GameController gameController;
    private float lastSpawnTime = 0;
    private void FixedUpdate()
    {
        if (gameController.startGame)
        {
            lastSpawnTime += Time.fixedDeltaTime;
            if (lastSpawnTime >= itemCreateFrequency)
            {
                SpawnItem();
                lastSpawnTime = 0;
            }
        }
    }

    private void SpawnItem()
    {
        var playerCount = GameObject.FindObjectsOfType<CharacterContorl>().ToList().Count;
        var spawnCount = Mathf.Max(1, Mathf.Ceil(playerCount / 2));
        var points = new List<Vector3>();
        for(int i = 0; i < itemPosition.transform.childCount; i++)
        {
            points.Add(itemPosition.transform.GetChild(i).position);
        }

        for (int i = 0; i < spawnCount; i++)
        {
            var rollIndex = UnityEngine.Random.Range(0, points.Count);
            var position = points[rollIndex];
            points.RemoveAt(rollIndex);
            var rollItemIndex = UnityEngine.Random.Range(0, itemList.Count);
            Instantiate(itemList[rollItemIndex], position, Quaternion.identity);
        }
    }

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

    public static ItemBuffBase CreatItemBuffByItemID(float itemID,CharacterContorl character)
    {
        switch (itemID)
        {
            case 1:
                return new RocketThrusterBuff(character);
            case 2:
                return new LargementPotionBuff(character);
            case 4:
                return new StickyBombBuff(character);
            default:
                UnityEngine.Debug.LogError("没有找到对应ID");
                return null;

        }


    }

    public static GameObject CreatTrapItemByItemID(float itemID, CharacterContorl character)
    {
        switch (itemID)
        {
            case 3:
                var bananaTrap = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemTrap/BananaTrap"));
                bananaTrap.GetComponent<ItemTrapBase>().Init(character);
                return bananaTrap;
            case 4:
                var stickyBombTrap = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemTrap/StickyBombTrap"));
                stickyBombTrap.GetComponent<ItemTrapBase>().Init(character);
                return stickyBombTrap;
            default:
                Debug.LogError("没有找到对应ID");
                return null;
        }
    }

    public static ItemProjectileBase CreatProjectileByItemID(float itemID, CharacterContorl character, Vector3 originalPosition)
    {
        ItemProjectileBase itemProjectbase = null;
        GameObject itemObj = null;
        switch (itemID)
        {
            case 1:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemProjectile/RocketProjectile"));
                break;
            case 4:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemProjectile/StickyBombProjectile"));

                break;
            case 5:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemProjectile/RopeProjectile/RopeProjectile"));
                break;

            default:
                Debug.LogError("没有找到对应ID");
                break;
        }

        if (itemObj != null)
        {
            itemObj.transform.position = originalPosition;
            itemProjectbase = itemObj.GetComponent<ItemProjectileBase>();
            itemProjectbase.Init(character);
        }
        return itemProjectbase;
    }

}
