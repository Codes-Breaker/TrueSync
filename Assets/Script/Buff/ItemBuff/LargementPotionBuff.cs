using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargementPotionBuff : ItemBuffBase
{
    //持续时间
    
    //放大倍数
    private float scaleParameter = 1.5f;
    //打击加成
    private float hitKnockBackToOhterArgument = 2f;

    //自身击退加成
    private float hitKnockBackToSelfArgument = 0.2f; 

    

    public LargementPotionBuff(CharacterContorl target) : base(target)
    {
        buffTime = 30f;
    }

    public LargementPotionBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {
        
    }

    public override void OnBuffApply()
    {
        character.transform.localScale = new Vector3(character.transform.localScale.x * scaleParameter, character.transform.localScale.y * scaleParameter, character.transform.localScale.z * scaleParameter);
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument * hitKnockBackToOhterArgument;
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument * hitKnockBackToSelfArgument;

        character.hitMaxDistance = character.hitMaxDistance * 2;
        base.OnBuffApply();
    }

    public override void OnBuffRemove()
    {
        character.transform.localScale = new Vector3(character.transform.localScale.x / scaleParameter, character.transform.localScale.y / scaleParameter, character.transform.localScale.z / scaleParameter);
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument / hitKnockBackToOhterArgument;
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument / hitKnockBackToSelfArgument;
        character.hitMaxDistance = character.hitMaxDistance / 2;
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