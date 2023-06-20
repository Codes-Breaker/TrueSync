using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketThrusterBuff : ItemBuffBase
{
    //推进提速时间
    private float runSpeedUpTime = 0.5f;
    //推进最大速度
    private float runMaxVelocity = 12f;

    GameObject rocket;
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
    }


    public override void OnBuffRemove()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.OnRocket, OnRocket);
        character.isInRocket = false;
        character.SetSnowCollider();
        character.anima.SetBool("inRocket", false);

        base.OnBuffRemove();
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) + (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * 1);
        //创建火箭
        var itemBase = ItemManager.CreatProjectileByItemID(1, character, point, Vector3.zero);
        (itemBase as RocketProjectile).itemRot = rocket.transform.rotation;
        (itemBase as RocketProjectile).rocketForward = rocket.transform.forward;

        rocket.gameObject.SetActive(false);
        GameObject.Destroy(rocket);
    }

    private void GenerateRocket()
    {

    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
        if (character.isDead || character.isStun)
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
                character.ridbody.AddForce(moveTarget * forceMagnitude - gravityDivide, ForceMode.Force);
        }
        //补偿重力分量
    }

    public override void OnCollide(Collision collision)
    {
        base.OnCollide(collision);
    }
}