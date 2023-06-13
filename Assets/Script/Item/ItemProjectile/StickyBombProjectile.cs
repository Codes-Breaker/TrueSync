using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombProjectile : ItemProjectileBase
{
    //本地爆炸倒计时总时长
    public float explosionMaxTime= 20f;

    private string stickyBombTrapPath = "Prefabs/Item/ItemTrap/StickyBombTrap";
    public override void Init(CharacterContorl character, Vector3 project)
    {
        base.Init(character,project);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        var otherCharacter = collision.collider.GetComponent<CharacterContorl>();
        if (otherCharacter)
        {
            var bombBuff = new StickyBombBuff(otherCharacter);
            bombBuff.SetExplosionTime(explosionMaxTime, explosionMaxTime);
            otherCharacter.OnGainBuff(bombBuff);
            OnEnd();
        }
    }

    protected override void OnTouchGround()
    {
        base.OnTouchGround();
        var trapObject = GameObject.Instantiate(Resources.Load<GameObject>(stickyBombTrapPath));  
        var trap = trapObject.GetComponent<StickyBombTrap>();
        if(trap)
        {
            trap.Init(character);
            trap.SetExplosionTime(explosionMaxTime);
            trapObject.transform.position = transform.position;
        }
        OnEnd();
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }
}
