using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapItemAbility : ItemAbilityBase
{

    //向后扔道具的位置偏移
    protected float offset = 2f;
    public TrapItemAbility(CharacterContorl character, ItemData data) : base(character, data)
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

    protected override void itemAbility()
    {
        var trapObject = ItemManager.CreatTrapItemByItemID(itemData.itemId, character);
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) - (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * offset);
        trapObject.transform.position = new Vector3(point.x, point.y - (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x,point.z);
        base.itemAbility();
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
