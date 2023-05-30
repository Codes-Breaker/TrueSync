using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargementPotionBuff : ItemBuffBase
{
    public LargementPotionBuff(CharacterContorl target):base(target)
    {

    }

    public LargementPotionBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }

    public override void OnBuffApply()
    {
        base.OnBuffApply();
    }

    public override void OnBuffRemove()
    {
        base.OnBuffRemove();
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
    }

    public override void OnCollide(Collision collision)
    {
        base.OnCollide(collision);
    }
}
