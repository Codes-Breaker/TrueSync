using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketThrusterBuff : ItemBuffBase
{
    //推进提速时间
    private float runSpeedUpTime = 1f;
    //推进最大速度
    private float runMaxVelocity = 12f;


    public RocketThrusterBuff(CharacterContorl target) : base(target)
    {
        buffTime = 10f;
    }

    public RocketThrusterBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
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