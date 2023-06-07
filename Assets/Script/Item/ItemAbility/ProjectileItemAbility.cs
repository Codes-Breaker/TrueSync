using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileItemAbility : ItemAbilityBase
{
    public ProjectileItemAbility(CharacterContorl character, ItemData data) : base(character, data)
    {
    }

    public override void Init()
    {
        base.Init();
    }

    public override void UseItemAbility()
    {
        base.UseItemAbility();
    }

    protected override void OnItemReduced()
    {
        base.OnItemReduced();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void End()
    {
        base.End();
    }
}
