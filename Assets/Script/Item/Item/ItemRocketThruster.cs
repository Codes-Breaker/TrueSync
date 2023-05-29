using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRocketThruster : ItemBase
{
    [Header("BUFF持续时间")]
    public float buffTime;
    protected override ItemBuffBase CreatItemBuff(CharacterContorl target)
    {
        return new RocketThrusterBuff(target,buffTime);
    }
}
