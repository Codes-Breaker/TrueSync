using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        buffTime = 6f;
    }

    public LargementPotionBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {
        
    }

    public override void OnBuffApply()
    {
        character.transform.DOScale(new Vector3(character.transform.localScale.x * scaleParameter, character.transform.localScale.y * scaleParameter, character.transform.localScale.z * scaleParameter), 0.5f).SetEase(Ease.OutBounce);
        //character.transform.localScale = new Vector3(character.transform.localScale.x * scaleParameter, character.transform.localScale.y * scaleParameter, character.transform.localScale.z * scaleParameter);
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument * hitKnockBackToOhterArgument;
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument * hitKnockBackToSelfArgument;

        character.hitMaxDistance = character.hitMaxDistance * 2;
        base.OnBuffApply();

        var eventObjectPrefab = Resources.Load<GameObject>("Prefabs/Effect/Huge");
        var eventObjectGameObject = GameObject.Instantiate(eventObjectPrefab, new Vector3(character.transform.position.x, character.transform.position.y - 1.5f, character.transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
    }

    public override void OnBuffRemove()
    {
        character.transform.DOScale(new Vector3(character.transform.localScale.x / scaleParameter, character.transform.localScale.y / scaleParameter, character.transform.localScale.z / scaleParameter), 0.5f).SetEase(Ease.OutBounce);
        //character.transform.localScale = new Vector3(character.transform.localScale.x / scaleParameter, character.transform.localScale.y / scaleParameter, character.transform.localScale.z / scaleParameter);
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument / hitKnockBackToOhterArgument;
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument / hitKnockBackToSelfArgument;
        character.hitMaxDistance = character.hitMaxDistance / 2;
        base.OnBuffRemove();

        var eventObjectPrefab = Resources.Load<GameObject>("Prefabs/Effect/Huge");
        var eventObjectGameObject = GameObject.Instantiate(eventObjectPrefab, new Vector3(character.transform.position.x, character.transform.position.y - 1.5f, character.transform.position.z), Quaternion.Euler(new Vector3(0, 0, 0)));
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