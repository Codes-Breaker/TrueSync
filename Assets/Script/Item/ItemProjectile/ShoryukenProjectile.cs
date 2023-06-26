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
    private List<CharacterContorl> characterContorls = new List<CharacterContorl>();
    InvulernableBuff buff;
    public override void Init(CharacterContorl character, Vector3 project)
    {
        base.Init(character, project);
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenEnd, DestroyShoryukenprojectile);
        stunBuffToSelf = new StunBuff(character);
        character.OnGainBuff(stunBuffToSelf);
        bodyCollider.isTrigger = true;

        buff = new InvulernableBuff(character, -1);
        character.OnGainBuff(buff);
    }

    public override void Launch()
    {
        base.Launch();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        rb.velocity = new Vector3(character.ridbody.velocity.x,rb.velocity.y,character.ridbody.velocity.z);
    }

    private void DestroyShoryukenprojectile()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenEnd, DestroyShoryukenprojectile);
        stunBuffToSelf.isEnd = true;
        buff.Finish();
        OnEnd();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var otherCharacter = other.GetComponent<CharacterContorl>();
        if (otherCharacter && otherCharacter != character && !characterContorls.Contains(otherCharacter))
        {
            otherCharacter.AddForce(Vector3.up * upForceArgument * otherCharacter.ridbody.mass,ForceMode.Force);
            var knockBackTarget = (otherCharacter.ridbody.transform.position - character.ridbody.transform.position).normalized;
            otherCharacter.AddForce(knockBackTarget * knockBackForceArgument * otherCharacter.ridbody.mass, ForceMode.Force);
            var stunBuffToOther = new StunBuff(otherCharacter,stunTime);
            otherCharacter.OnGainBuff(stunBuffToOther);

            var eventObjectPrefab = Resources.Load<GameObject>("Prefabs/Effect/StunHit");
            var eventObjectGameObject = Instantiate(eventObjectPrefab, other.transform.position + (transform.position - other.transform.position).normalized * (otherCharacter.bodyCollider as SphereCollider).radius * otherCharacter.transform.localScale.y, Quaternion.Euler(new Vector3(0, 0, 0)));

            //打击角度计算
            var hitOnPlane = Vector3.ProjectOnPlane((rb.transform.position - otherCharacter.ridbody.position), otherCharacter.groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(otherCharacter.ridbody.transform.forward, otherCharacter.groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, otherCharacter.groundNormal);
            otherCharacter.anima.SetFloat("hitAngle", hitAngle);
            otherCharacter.anima.SetBool("isHit", true);

            characterContorls.Add(otherCharacter);
        }

    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
