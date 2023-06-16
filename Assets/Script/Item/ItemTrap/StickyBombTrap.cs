using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBombTrap : ItemTrapBase
{
    //本地爆炸倒计时总时长
    public float explosionMaxTime;
    //本地爆炸倒计时
    public float explosionTime;
    //爆炸特效预制体路径
    private string timeLapseBombExplosionEffectPath = "Prefabs/Effect/StickyBomb_Explosion";
    //爆炸力大小
    public float explosionForceArgument;

    public float explosionRangeRadius;

    public Transform stickyBombFuseGameObject;

    private bool isCreatCountDownEffect = false;
    private string timeLapseBombCountDownEffectPath = "Prefabs/Effect/BombCountDown";

    public override void Init(CharacterContorl character)
    {
        base.Init(character);

    }

    public void SetExplosionTime(float maxTime)
    {
        explosionTime = maxTime;
        explosionMaxTime = maxTime;
    }
    private void Update()
    {
        explosionTime -= Time.deltaTime;
        if (explosionTime <= 0)
            Explosion();
        else
        {
            SetFuseState();

        }
    }


    private void SetFuseState()
    {
        if (explosionTime < explosionMaxTime / 4 && !isCreatCountDownEffect)
        {
            var effectPrefab = Resources.Load(timeLapseBombCountDownEffectPath);
            GameObject.Instantiate(effectPrefab,transform);
            isCreatCountDownEffect = true;
        }
        stickyBombFuseGameObject.localScale = new Vector3(stickyBombFuseGameObject.localScale.x, explosionTime / explosionMaxTime, stickyBombFuseGameObject.localScale.z);
    }
    private void Explosion()
    {
        var colliders = GameObject.FindObjectsOfType<Rigidbody>();

        if (colliders.Length != 0)
        {
            foreach (var item in colliders)
            {
                if ((item.transform.position - transform.position).magnitude < explosionRangeRadius)
                {
                    item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument,transform.position, explosionRangeRadius);
                }
            }

        }
        var effectPrefab = Resources.Load(timeLapseBombExplosionEffectPath);
        var effectGameObject = (GameObject)GameObject.Instantiate(effectPrefab, transform.position, Quaternion.Euler(Vector3.zero));
        GameObject.Destroy(effectGameObject, 3f);
        base.OnEnd();

    }

    private void OnTriggerEnter(Collider collision)
    {
        var otherCharacter = collision.gameObject.GetComponent<CharacterContorl>();
        if (otherCharacter)
        {
            var bombBuff = new StickyBombBuff(otherCharacter);
            bombBuff.SetExplosionTime(explosionTime,explosionMaxTime);
            otherCharacter.OnGainBuff(bombBuff);
            base.OnEnd();
        }
    }
}
