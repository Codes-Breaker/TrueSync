using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MudBun;
using DG.Tweening.Core;

public class RocketThrusterBuff : ItemBuffBase
{
    //推进提速时间
    private float runSpeedUpTime = 0.5f;
    //推进最大速度
    private float runMaxVelocity = 12f;
    public ParticleSystem particle;
    GameObject rocket;
    private Light light;
    private MudNoiseVolume volume;
    //自身击退加成
    private float hitKnockBackToSelfArgument = 1f;
    //打击加成
    private float hitKnockBackToOhterArgument = 1f;
    public RocketThrusterBuff(CharacterContorl target) : base(target)
    {
        buffTime = 15f;
    }

    public RocketThrusterBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {
    }
    public override void OnBuffApply()
    {
        base.OnBuffApply();

        foreach(var buff in character.getRocketThrusterBuffs())
        {
            if (buff != this)
            {
                buff.Finish();
            }
        }

        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.OnRocket, OnRocket);
        character.anima.SetBool("inRocket", true);
        character.isInRocket = true;
        character.SetSnowCollider();
        character.anima.SetTrigger("isInRocket");
    }

    public void OnRocket()
    {
        var rocketPrefab = Resources.Load<GameObject>("Prefabs/Item/ItemOnCharacter/RocketOnCharacterUsed");
        rocket = GameObject.Instantiate(rocketPrefab, this.character.itemPlaceBelly);
        rocket.transform.localPosition = new Vector3(0, 0, 0);
        rocket.gameObject.SetActive(true);
        rocket.transform.localPosition = new Vector3(0, 0, 0);
        particle = rocket.GetComponentInChildren<ParticleSystem>();
        light = rocket.GetComponentInChildren<Light>();
        light.intensity = 0;
        light.DOIntensity(1, 2);
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument * hitKnockBackToSelfArgument;
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument * hitKnockBackToOhterArgument;
        volume = rocket.GetComponentInChildren<MudNoiseVolume>();
        var getter = new DOGetter<float>(() =>
        {
            return volume.Threshold;
        });
        var setter = new DOSetter<float>((x) => {
            volume.Threshold = x;
        });
        volume.Threshold = 1;
        DOTween.To(getter, setter, 0.2f, 1);
    }


    public override void OnBuffRemove()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.OnRocket, OnRocket);
        character.isInRocket = false;
        character.SetSnowCollider();
        if (character.countRocketBuff() == 1)
            character.anima.SetBool("inRocket", false);
        character.hitKnockbackToSelfArgument = character.hitKnockbackToSelfArgument / hitKnockBackToSelfArgument;
        character.hitKnockBackToOtherArgument = character.hitKnockBackToOtherArgument / hitKnockBackToOhterArgument;
        base.OnBuffRemove();
        GenerateRocket();
        //rocket.gameObject.SetActive(false);
        //GameObject.Destroy(rocket);
    }

    private void GenerateRocket()
    {
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) + (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * 1);
        //创建火箭
        rocket.transform.parent = null;
        var itemBase = rocket.AddComponent<RocketProjectile>();
        (itemBase as RocketProjectile).itemRot = rocket.transform.rotation;
        Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);
        (itemBase as RocketProjectile).rocketForward = rotation * rocket.transform.forward;
        itemBase.Init(character, Vector3.zero);
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();

        if (character.isDead || character.HasQTEStun())
        {
            base.Finish();
            return;

        }
        if (character.ridbody.velocity.magnitude < runMaxVelocity * 0.96f)
        {
            var acceleration = runMaxVelocity / runSpeedUpTime;
            var forceMagnitude = character.ridbody.mass * acceleration;
            var gravityDivide = Vector3.zero;
            if (character.isTouchingSlope || character.isGrounded)
            {
                gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, character.groundNormal) * character.ridbody.mass;
                var gravityFrictionDivide = Physics.gravity - gravityDivide;
                var frictionForceMagnitude = character.ridbody.mass * character.bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
                forceMagnitude = forceMagnitude + frictionForceMagnitude;
            }

            var moveTarget = character.ridbody.transform.forward;
            moveTarget = moveTarget.normalized;
            moveTarget = Vector3.ProjectOnPlane(moveTarget, character.groundNormal).normalized;
            if (!character.hasStunBuff())
                character.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
        }
        //补偿重力分量
    }

    public override void OnCollide(Collision collision)
    {
        base.OnCollide(collision);
        if (collision.gameObject.CompareTag("StaticObject"))
        {
            var hitOnPlane = Vector3.ProjectOnPlane((collision.contacts[0].point - character.ridbody.position), character.groundNormal).normalized;
            var forwardOnPlane = Vector3.ProjectOnPlane(character.ridbody.transform.forward, character.groundNormal).normalized;
            var hitAngle = Vector3.SignedAngle(forwardOnPlane, hitOnPlane, character.groundNormal);
            if (Mathf.Abs(hitAngle) <= 45)
            {
                this.Finish();
            }
            
        }
    }
}