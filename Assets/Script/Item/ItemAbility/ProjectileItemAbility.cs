using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileItemAbility : ItemAbilityBase
{
    protected float offset = 1f;
    private ItemProjectileBase itemBase;
    public ProjectileItemAbility(CharacterContorl character, ItemData data) : base(character, data)
    {
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.ThrowBoom, Launch);
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

    protected override void itemAbility()
    {
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) + (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * offset);
        //trapObject.transform.position = new Vector3(point.x, point.y - (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x, point.z);
        //trapObject.transform.position = point;
        itemBase = ItemManager.CreatProjectileByItemID(itemData.itemId, character, point);
        itemBase.Throw();
        Throw();
        base.itemAbility();
    }

    private void Throw()
    {
        character.anima.SetTrigger("throwBoom");
    }

    private void Launch()
    {
        itemBase.Launch();
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
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ThrowBoom, Throw);
        base.End();
    }
}
