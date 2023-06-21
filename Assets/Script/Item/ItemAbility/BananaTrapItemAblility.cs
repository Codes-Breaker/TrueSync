using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BananaTrapItemAblility : TrapItemAbility
{
    private GameObject itemOnHand;

    private float bananaFlyHight = 2f;
    private float bananaFlyTime = 0.5f; 
    private string bananaOnHandBeforePrefabPath = "Prefabs/Item/ItemOnHand/BananaOnHandBefore";
    private string bananaOnHandAfterPrefabPath = "Prefabs/Item/ItemOnHand/BananaOnHandAfter";
    public BananaTrapItemAblility(CharacterContorl character, ItemData data) : base(character, data)
    {

    }

    public override void Init()
    {
        base.Init();
    }

    protected override void itemAbility()
    {
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.BananaTake,TakeBanana);
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.BananaEat, EatBanana);
        character.animationEventReceiver.RegisterEvent(AnimationEventReceiver.EventEnum.BananaThrow, ThrowBanana);
        character.anima.SetTrigger("throwBanana");
        base.itemAbility();
    }

    protected override void CreatTarpItem()
    {
        
    }

    private void TakeBanana()
    {
        itemOnHand = GameObject.Instantiate(Resources.Load<GameObject>(bananaOnHandBeforePrefabPath));
        itemOnHand.transform.SetParent(character.itemPlaceHand);
        itemOnHand.transform.localPosition = Vector3.zero;
        itemOnHand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.BananaTake, TakeBanana);
    }

    private void EatBanana()
    {
        GameObject.Destroy(itemOnHand);
        itemOnHand = GameObject.Instantiate(Resources.Load<GameObject>(bananaOnHandAfterPrefabPath));
        itemOnHand.transform.SetParent(character.itemPlaceHand);
        itemOnHand.transform.localPosition = Vector3.zero;
        itemOnHand.transform.localRotation = Quaternion.Euler(Vector3.zero);
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.BananaEat, EatBanana);
    }

    private void ThrowBanana()
    {
        character.animationEventReceiver.UnRegisterEvent(AnimationEventReceiver.EventEnum.BananaThrow, ThrowBanana);
        itemOnHand.transform.SetParent(null);
        var point = (character.transform.position + (character.bodyCollider as SphereCollider).center) - (character.ridbody.transform.forward.normalized * (character.bodyCollider as SphereCollider).radius * character.transform.localScale.x + character.ridbody.transform.forward.normalized * offset);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(itemOnHand.transform.DOPath(new Vector3[] { itemOnHand.transform.position, (point + itemOnHand.transform.position) / 2 + new Vector3(0, bananaFlyHight, 0), point }, bananaFlyTime, PathType.Linear))
            .Join(itemOnHand.transform.DOScale(new Vector3(2.6f,2.6f,2.6f),bananaFlyTime))
            .Join(itemOnHand.transform.GetChild(0).DOLocalRotate(new Vector3(-90,0,0),bananaFlyTime))
            .Join(itemOnHand.transform.GetChild(0).DOLocalMove(Vector3.zero,bananaFlyTime))
            .Join(itemOnHand.transform.DORotate(Vector3.zero,bananaFlyTime))
            .OnComplete(() => { 
                var trapObject = ItemManager.CreatTrapItemByItemID(itemData.itemId, character);
                trapObject.transform.rotation = itemOnHand.transform.rotation;
                trapObject.transform.position = point;
                GameObject.Destroy(itemOnHand);
            }
            ) ;
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void End()
    {
        base.End();
    }

}
