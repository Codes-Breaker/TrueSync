using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BananaTrap : ItemTrapBase
{
    //—£‘Œ ±º‰
    private float stunTime = 2f;
    private float rollTimes = 3;

    public override void Init(CharacterContorl character)
    {
        base.Init(character);
        character.currentStamina = character.maxActorStamina;
    }

    private void OnTriggerEnter(Collider collision)
    {
        var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
        if (otherCollision && !otherCollision.invulernable && !otherCollision.isInRocket)
        {
            var buff = new SliperyBuff(otherCollision, stunTime);
            otherCollision.OnGainBuff(buff);
            otherCollision.anima.SetBool("isRotating", true);
            var rotateLeft = false;
            otherCollision.anima.SetFloat("angularVelocityY", rotateLeft? 1 : -1);
            otherCollision.IKObject.transform.DOKill();
            otherCollision.IKObject.transform.DOLocalRotate(new Vector3(otherCollision.IKObject.transform.localRotation.eulerAngles.x, (rotateLeft ? -1 : 1) * 360 * rollTimes, otherCollision.IKObject.transform.localRotation.eulerAngles.z), stunTime, RotateMode.FastBeyond360)
                .SetEase(Ease.OutCubic).onComplete +=(()=> {
                    otherCollision.anima.SetBool("isRotating", false);
                });
            base.OnEnd();
        }
    }
}
