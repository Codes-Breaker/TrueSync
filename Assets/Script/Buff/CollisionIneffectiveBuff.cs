using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class CollisionIneffectiveBuff : Buff
{
    public CollisionIneffectiveBuff(CharacterContorl target) : base(target)
    {
        buffTime = 0.1f;
    }

    public CollisionIneffectiveBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }
}

