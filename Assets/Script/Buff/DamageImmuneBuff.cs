using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class DamageImmuneBuff : Buff
{
    public DamageImmuneBuff(CharacterContorl target) : base(target)
    {
        buffTime = 0.1f;
    }

    public DamageImmuneBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }
}

