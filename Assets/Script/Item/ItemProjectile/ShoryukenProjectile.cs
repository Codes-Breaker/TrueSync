using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoryukenProjectile : ItemProjectileBase
{
    private StunBuff stunBuffToSelf;
    [Header("击晕时间")]
    public float stunTime;
    [Header("向上打击力参数(无视重力)")]
    public float upForceArgument;
    [Header("击退力参数(无视重力)")]
    public float knockBackForceArgument;

    public override void Init(CharacterContorl character, Vector3 project)
    {
        base.Init(character, project);
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenEnd, DestroyShoryukenprojectile);
        stunBuffToSelf = new StunBuff(character);
        character.OnGainBuff(stunBuffToSelf);
        bodyCollider.isTrigger = true;
    }

    public override void Launch()
    {
        base.Launch();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private void DestroyShoryukenprojectile()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenEnd, DestroyShoryukenprojectile);
        stunBuffToSelf.isEnd = true;
        OnEnd();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var otherCharacter = other.GetComponent<CharacterContorl>();
        if (otherCharacter && otherCharacter != character)
        {
            otherCharacter.ridbody.AddForce(Vector3.up * upForceArgument * otherCharacter.ridbody.mass,ForceMode.Force);
            var knockBackTarget = (otherCharacter.ridbody.transform.position - character.ridbody.transform.position).normalized;
            otherCharacter.ridbody.AddForce(knockBackTarget * knockBackForceArgument * otherCharacter.ridbody.mass, ForceMode.Force);
            var stunBuffToOther = new StunBuff(otherCharacter,stunTime);
            otherCharacter.OnGainBuff(stunBuffToOther); 
        }

    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
