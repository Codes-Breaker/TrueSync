using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileItemAbility : ItemAbilityBase
{
    protected float offset = 1f;
    private ItemProjectileBase itemBase;
    private GameObject itemOnHand;
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

    protected override void itemAbility()
    {
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) + (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * offset);
        //trapObject.transform.position = new Vector3(point.x, point.y - (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x, point.z);
        //trapObject.transform.position = point;
        
        Throw();
        base.itemAbility();
    }

    private void Throw()
    {
        itemOnHand = ItemManager.CreatItemOnHand(itemData.itemId);
        itemOnHand.transform.SetParent(character.itemPlaceHand);
        itemOnHand.transform.localPosition = Vector3.zero;
        //Physics.IgnoreCollision(this.bodyCollider, character.bodyCollider, true);
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.ThrowBoom, Launch);
        character.anima.SetTrigger("throwBoom");
    }

    private void Launch()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.ThrowBoom,Launch);
        GameObject.Destroy(itemOnHand);
        itemBase = ItemManager.CreatProjectileByItemID(itemData.itemId, character, itemOnHand.transform.position,character.transform.forward);
        //itemBase.Launch();
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
