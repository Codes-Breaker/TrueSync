using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SliperyBuff : StunBuff
{
    public SliperyBuff(CharacterContorl target) : base(target)
    {
        buffTime = 1f;
    }

    public SliperyBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }
    public const float frictionDamp = 0.1f;
    public override void OnBuffUpdate()
    {
        var gravityDivide = Vector3.zero;
        if (this.character.isTouchingSlope || this.character.isGrounded)
        {
            gravityDivide = Vector3.ProjectOnPlane(Physics.gravity, character.groundNormal) * character.ridbody.mass;
            var gravityFrictionDivide = Physics.gravity - gravityDivide;
            var frictionForceMagnitude = character.ridbody.mass * character.bodyCollider.material.dynamicFriction * gravityFrictionDivide.magnitude;
            var force = frictionForceMagnitude * frictionDamp;
            var moveTarget = character.ridbody.velocity.normalized;
            character.ridbody.AddForce(moveTarget * force - gravityDivide, ForceMode.Force);
        }
        base.OnBuffUpdate();
    }
}

