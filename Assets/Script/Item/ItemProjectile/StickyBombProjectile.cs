using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombProjectile : ItemProjectileBase
{
    public override void Init(CharacterContorl character, Vector3 project)
    {
        base.Init(character,project);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }




    public override void OnEnd()
    {
        base.OnEnd();
    }
}
