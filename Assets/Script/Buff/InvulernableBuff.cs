using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class InvulernableBuff : Buff
{
    public InvulernableBuff(CharacterContorl target) : base(target)
    {
        buffTime = 1f;
    }

    public InvulernableBuff(CharacterContorl target, float buffTime) : base(target, buffTime)
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
}

