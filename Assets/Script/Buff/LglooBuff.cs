using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LglooBuff : StunBuff
{
    private float maxSpeed = 10;
    public LglooBuff(CharacterContorl target) : base(target)
    {
        buffTime = 1.5f;
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
        var releaseDir = character.ridbody.transform.forward;
        releaseDir = releaseDir.normalized;


        Vector3 vel1 = character.velocityBeforeCollision;

        var d1 = Vector3.Angle(vel1, releaseDir);

        var degree1 = d1 * Mathf.Deg2Rad;
        var m1 = (Mathf.Cos(degree1) * vel1).magnitude;

        if (m1 < maxSpeed)
        {
            character.AddForce(releaseDir * 40, ForceMode.Impulse);
        }
    }

    public override void OnBuffRemove()
    {
        base.OnBuffRemove();
        character.ridbody.velocity = Vector3.zero;
    }


    public override void OnCollide(Collision collision)
    {

        base.OnCollide(collision);
        if (collision.transform.GetComponent<CharacterContorl>())
        {
            this.Finish();
        }
        else if (collision.transform.tag == "Wall")
        {
            this.Finish();
        }

    }

}
