using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTrap : ItemTrapBase
{
    //—£‘Œ ±º‰
    private float stunTime = 1f;

    private void OnTriggerEnter(Collider collision)
    {
        var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
        if (otherCollision)
        {
            var buff = new SliperyBuff(otherCollision, stunTime);
            otherCollision.OnGainBuff(buff);
            base.OnEnd();
        }
    }
}
