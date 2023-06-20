using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class QTEBuff : StunBuff
{

    //连击加成时间
    public float QTEAccelerateRate = 0.1f;
    //QTE优先级
    public int QTEPriority = 0;
    //是否需要关闭IK
    public bool requireIKOff = false;
    //QTE使用Button
    public QTEButton button = QTEButton.A;

    public QTEBuff(CharacterContorl target) : base(target)
    {

    }

    public QTEBuff(CharacterContorl target, float buffTime, float qteRate) : base(target, buffTime)
    {
        QTEAccelerateRate = qteRate;
    }

    public void PressButton(QTEButton button)
    {
        if (button == QTEButton.A)
        {
            buffTime -= QTEAccelerateRate;
        }
    }
}

