using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombBuff : ItemBuffBase
{
    private string stickyBombPrefabPath;

    private float explosionTime;

    private float explosionForceArgument;
    public StickyBombBuff(CharacterContorl target) : base(target)
    {
    }

    public StickyBombBuff(CharacterContorl target, float buffTime): base(target , buffTime)
    {
    }

    public override void OnBuffApply()
    {
        base.OnBuffApply();
        var stickyBombPrefab = Resources.Load(stickyBombPrefabPath);
        var stickyBombGameObject = GameObject.Instantiate(stickyBombPrefab);
    }

    public void SetExplosionTime(float time)
    {
        explosionTime = time;
    }
    public override void OnBuffRemove()
    {
        base.OnBuffRemove();
    }

    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
        explosionTime -= Time.deltaTime;
        if (explosionTime <= 0)
            Explosion();
    }
    
    private void Explosion()
    {

    }
    public override void OnCollide(Collision collision)
    {
        base.OnCollide(collision);
        var otherCharacter = collision.collider.GetComponent<CharacterContorl>();
        if (otherCharacter)
        {
            var bombBuff = new StickyBombBuff(otherCharacter);
            bombBuff.SetExplosionTime(explosionTime);
            otherCharacter.OnGainBuff(bombBuff);
            base.Finish();
        }
    }
}
