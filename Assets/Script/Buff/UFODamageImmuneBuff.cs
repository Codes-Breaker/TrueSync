using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class UFODamageImmuneBuff : DamageImmuneBuff
{
    public UFODamageImmuneBuff(CharacterContorl target) : base(target)
    {
        buffTime = 0.1f;
    }

    public UFODamageImmuneBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
    {

    }
}

