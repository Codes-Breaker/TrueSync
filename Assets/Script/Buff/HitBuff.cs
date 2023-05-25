﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HitBuff : StunBuff
{
    public HitBuff(CharacterContorl target) : base(target)
    {
        buffTime = 0.1f;
    }

    public HitBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }

    public override void OnBuffApply()
    {
        base.OnBuffApply();
        character.ridbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
       // Debug.LogError($"isonground: {this.character.isGrounded} ==> {this.character.ridbody.velocity}");
    }

    public override void OnBuffRemove()
    {
        base.OnBuffRemove();
        character.ridbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
}
