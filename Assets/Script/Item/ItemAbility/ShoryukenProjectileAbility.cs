using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoryukenProjectileAbility : ProjectileItemAbility
{
    public ShoryukenProjectileAbility(CharacterContorl character, ItemData data) : base(character, data)
    {

    }

    public override void Init()
    {
        base.Init();
    }
    protected override void itemAbility()
    {
        base.itemAbility();
    }

    protected override void Throw()
    {
        character.anima.SetTrigger("shenglong");
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenStart, Launch);
    }

    protected override void Launch()
    { 
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ShoryukenStart, Launch);
        itemBase = ItemManager.CreatProjectileByItemID(itemData.itemId, character, character.transform.position, character.transform.up);
    }

    public override void End()
    {
        base.End();
    }

}
