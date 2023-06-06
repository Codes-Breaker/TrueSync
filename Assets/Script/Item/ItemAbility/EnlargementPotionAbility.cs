using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnlargementPotionAbility : ItemAbilityBase
{
    public EnlargementPotionAbility(CharacterContorl character, ItemData data):base(character , data)
    {
    }

    public override void Init()
    {
        base.Init();
    }

    protected override void CloseItemAbility()
    {
        base.CloseItemAbility();
    }

    protected override void UseItemAbility()
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
