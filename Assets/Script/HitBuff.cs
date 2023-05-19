using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class HitBuff : StunBuff
{
    public HitBuff(CharacterContorl target) : base(target)
    {
        buffTimes = 0.1f;
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
       // Debug.LogError($"isonground: {this.character.isGrounded} ==> {this.character.ridbody.velocity}");
    }
}

