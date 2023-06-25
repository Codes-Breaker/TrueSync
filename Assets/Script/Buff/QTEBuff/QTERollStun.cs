using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//翻滚眩晕
public class QTERollStun : QTEBuff
{
    private bool recovering = false;
    //眩晕旋转状态相关
    Vector3 rollRotationAxis = Vector3.zero;
    float rollRotationAmount = 0f;
    private bool isRollContinue;
    public QTERollStun(CharacterContorl target) : base(target)
    {

    }

    public QTERollStun(CharacterContorl target, float buffTime, float qteRate) : base(target, buffTime, qteRate)
    {
        QTEAccelerateRate = qteRate;
        requireIKOff = true;
        QTEPriority = -1; //最高优先级
    }

    public override void OnBuffApply()
    {
        character.stun.gameObject.SetActive(true);
        character.anima.SetBool("isStun", true);
        base.OnBuffApply();
    }

    public override void OnBuffRemove()
    {
        if (character.countQTEStun() == 1)
        {
            character.stun.gameObject.SetActive(false);
            character.anima.SetBool("isStun", false);
        }
        base.OnBuffRemove();
    }

    public override void OnBuffUpdate()
    {
        if (recovering)
            return;
        //特殊流程，因为有个翻身
        if (buffTime != -1)
        {
            buffTime = buffTime - Time.deltaTime;
            if (buffTime <= 0)
            {
                recovering = true;
                character.stunRecoverTime *= character.stunAccumulateTime;
                character.maxStunValue = Math.Max(character.stunMinValue, character.maxStunValue - character.stunDecreaseRate);
                character.currentStunValue = character.maxStunValue;
                character.IKObject.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f).onComplete += () =>
                {
                    Finish();
                };
            }
        }
    }

    public override void OnBuffLateUpdate()
    {
        base.OnBuffLateUpdate();
        if (recovering)
            return;
        MoveRoll();
    }

    private void MoveRoll()
    {
        if (character.ridbody.velocity.magnitude > character.stunStopRollMinVelocity)
        {
            rollRotationAxis = -Vector3.Cross(character.groundNormal, character.ridbody.velocity);
            rollRotationAmount = character.ridbody.velocity.magnitude;
            character.IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.World);
            isRollContinue = true;
        }
        else
        {
            var upAngle = Vector3.Angle(character.IKObject.transform.up, character.groundNormal);
            var forwardAngle = Vector3.Angle(character.IKObject.transform.forward, character.groundNormal);
            isRollContinue = upAngle < character.stunStopRollMinAngle || (180f - upAngle) < character.stunStopRollMinAngle || (character.stunStopRollMaxAngle < upAngle && upAngle < (180 - character.stunStopRollMaxAngle)) && (character.stunStopRollMaxAngle < forwardAngle && forwardAngle < (180 - character.stunStopRollMaxAngle));
            //近乎停止旋转时的平衡补偿
            if (isRollContinue)
            {
                character.IKObject.transform.Rotate(rollRotationAxis, -rollRotationAmount, Space.World);
            }
        }
        
    }
}

