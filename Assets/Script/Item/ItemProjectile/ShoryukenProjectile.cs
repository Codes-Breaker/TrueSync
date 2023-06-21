using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoryukenProjectile : ItemProjectileBase
{
    private StunBuff stunBuffToSelf;
    public override void Init(CharacterContorl character, Vector3 project)
    {
        base.Init(character, project);

    }

    public override void Launch()
    {
        base.Launch();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
