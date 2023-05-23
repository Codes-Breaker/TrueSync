using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBuff : Buff
{
    public StunBuff(CharacterContorl target) : base(target)
    {

    }

    public StunBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }
}
