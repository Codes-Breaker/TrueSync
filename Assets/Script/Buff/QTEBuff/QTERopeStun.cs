using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class QTERopeStun : QTEBuff
{
    public QTERopeStun(CharacterContorl target) : base(target)
    {

    }

    public QTERopeStun(CharacterContorl target, float buffTime, float qteRate) : base(target, buffTime, qteRate)
    {
        QTEAccelerateRate = qteRate;
        requireIKOff = false;
        QTEPriority = 1; //最高优先级
    }

    public override void OnBuffApply()
    {
        character.stun.gameObject.SetActive(true);
        character.anima.SetBool("isStun", true);
        base.OnBuffApply();
    }

    public override void OnBuffRemove()
    {
        character.stun.gameObject.SetActive(false);
        character.anima.SetBool("isStun", false);
        base.OnBuffRemove();
    }
}

