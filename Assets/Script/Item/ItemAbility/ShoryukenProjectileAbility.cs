using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoryukenProjectileAbility : ProjectileItemAbility
{
    private float jumpForce = 45;
    public ShoryukenProjectileAbility(CharacterContorl character, ItemData data) : base(character, data)
    {

    }

    public override void Init()
    {
        base.Init();
        var boingBones = this.itemGameObject.GetComponent<BoingKit.BoingBones>();
        if (boingBones != null)
        {
            for (int i = 0; i < character.boingBones.BoingColliders.Length; i++)
            {
                boingBones.BoingColliders[i] = character.boingBones.BoingColliders[i];
            }
        }

    }
    protected override void itemAbility()
    {
        base.itemAbility();
    }

    protected override void Throw()
    {
        //��Ծ
        character.ridbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        character.anima.SetTrigger("shenglong");
        character.anima.SetBool("Jump", true);
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
