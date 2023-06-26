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

public enum EquipPlace 
{ 
    OnHead,
    OnHeadStatic,
    OnDorsal,
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
    public EquipPlace equipPlace;
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
        var players = GameObject.FindObjectsOfType<CharacterContorl>();
        var playerCount = players.Count();
        var playerWithUnsedAbility = players.Count(x => x.itemAbility != null);
        var spawnCount = Mathf.Max(1, Mathf.Ceil(playerCount / 2)) + 1;
        var points = new List<Vector3>();
        var pointTransform = new List<Transform>();
        var existCount = 0;
        for(int i = 0; i < itemPosition.transform.childCount; i++)
        {
            if (itemPosition.transform.GetChild(i).childCount == 0)
            {
                pointTransform.Add(itemPosition.transform.GetChild(i));
                points.Add(itemPosition.transform.GetChild(i).position);
            }
            else
            {
                existCount++;
            }
        }

        var requireSpawn = spawnCount - existCount - playerWithUnsedAbility;

        if (points.Count >= requireSpawn && requireSpawn > 0)
        {
            for (int i = 0; i < requireSpawn; i++)
            {
                var rollIndex = UnityEngine.Random.Range(0, points.Count);
                var position = points[rollIndex];
                points.RemoveAt(rollIndex);
                var rollItemIndex = UnityEngine.Random.Range(0, itemList.Count);
                Instantiate(itemList[rollItemIndex], position, Quaternion.identity, pointTransform[rollIndex]);
            }
        }

    }

    public static ItemAbilityBase CreatItemAbilityByItemData(ItemData itemData,CharacterContorl character)
    {
        switch(itemData.itemId)
        {
            case 1:
                return new BuffItemAbility(character, itemData);
            case 2:
                return new BuffItemAbility(character, itemData);
            case 3:
                return new BananaTrapItemAblility(character, itemData);
            case 4:
                return new ProjectileItemAbility(character, itemData);
            case 5:
                return new ProjectileItemAbility(character, itemData);
            case 6:
                return new ShoryukenProjectileAbility(character, itemData);
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

    public static GameObject CreatItemOnHand(float itemID)
    {
        GameObject itemObj = null;
        switch (itemID)
        {
            case 4:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemOnHand/StickyBombOnHand"));
                return itemObj;
            case 5:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemOnHand/RopeProjectileOnHand"));
                return itemObj;
            default:
                Debug.LogError("没有找到对应ID");
                return null;
        }
    }

    public static ItemProjectileBase CreatProjectileByItemID(float itemID, CharacterContorl character, Vector3 originalPosition,Vector3 project)
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
            case 6:
                itemObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Item/ItemProjectile/ShoryukenProjectile"));
                break;
            default:
                Debug.LogError("没有找到对应ID");
                break;
        }

        if (itemObj != null)
        {
            itemObj.transform.position = originalPosition;
            itemProjectbase = itemObj.GetComponent<ItemProjectileBase>();
            itemProjectbase.Init(character,project);
        }
        return itemProjectbase;
    }

}
