using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StickyBombBuff : ItemBuffBase
{
    //爆炸范围
    private float explosionRangeRadius = 10f;
    //爆炸力大小
    private float explosionForceArgument = 20f;
    //传递时的飞行高度
    public float stickyBombHeight = 5f;
    //传递时的飞行时间
    public float stickyBombFlyTime = 1f;
    //爆炸特效预制体路径
    private string timeLapseBombExplosionEffectPath = "Prefabs/Effect/StickyBomb_Explosion";
    //倒计时特效
    private string timeLapseBombCountDownEffectPath = "Prefabs/Effect/BombCountDown";
    //本地爆炸倒计时
    private float explosionTime;
    //本地爆炸倒计时总时长
    public float explosionMaxTime;

    private bool isCreatCountDownEffect = false;

    private Tweener pathTweener;

    private CharacterContorl otherCharacter;

    private bool isPassingBomb = false;
    private string stickyBombPrefabPath = "Prefabs/Item/ItemBuffPrefab/StickyBombBuffItem";
    private GameObject stickyBombGameObject;
    private Transform stickyBombFuseGameObject;
    public StickyBombBuff(CharacterContorl target) : base(target)
    {
    }

    public StickyBombBuff(CharacterContorl target, float buffTime): base(target , buffTime)
    {
    }

    public override void OnBuffApply()
    {
        base.OnBuffApply();
        var stickyBombPrefab = Resources.Load<GameObject>(stickyBombPrefabPath);
        stickyBombGameObject = GameObject.Instantiate(stickyBombPrefab,character.itemPlaceHead);
        stickyBombFuseGameObject = stickyBombGameObject.transform.Find("lead/root");
    }

    public void SetExplosionTime(float time,float maxTime)
    {
        explosionTime = time;
        explosionMaxTime = maxTime;
    }
    public override void OnBuffRemove()
    {
        base.OnBuffRemove();
        GameObject.Destroy(stickyBombGameObject);

    }

    
    public override void OnBuffUpdate()
    {
        base.OnBuffUpdate();
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
        if(explosionTime <explosionMaxTime/4 && !isCreatCountDownEffect)
        {
            var effectPrefab = Resources.Load(timeLapseBombCountDownEffectPath);
            GameObject.Instantiate(effectPrefab, stickyBombGameObject.transform);
            isCreatCountDownEffect = true;
        }
        stickyBombFuseGameObject.localScale = new Vector3(stickyBombFuseGameObject.localScale.x, explosionTime / explosionMaxTime, stickyBombFuseGameObject.localScale.z);
    }
    
    private void Explosion()
    {
        this.character.TakeStun(100);

        var colliders = GameObject.FindObjectsOfType<Rigidbody>();

        if (colliders.Length != 0)
        {
            foreach (var item in colliders)
            {
                if ((item.transform.position - stickyBombGameObject.transform.position).magnitude < explosionRangeRadius)
                {
                    item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument, stickyBombGameObject.transform.position, explosionRangeRadius);
                }
            }

        }
        var effectPrefab = Resources.Load(timeLapseBombExplosionEffectPath);
        var effectGameObject = (GameObject)GameObject.Instantiate(effectPrefab, stickyBombGameObject.transform.position, Quaternion.Euler(Vector3.zero));
        GameObject.Destroy(effectGameObject, 3f);
        base.Finish();

    }
    public override void OnCollide(Collision collision)
    {
        base.OnCollide(collision);
        var otherCharacter = collision.collider.GetComponent<CharacterContorl>();
        if (otherCharacter && !isPassingBomb)
        {
            this.otherCharacter = otherCharacter;
            stickyBombGameObject.transform.SetParent(null);
            pathTweener = stickyBombGameObject.transform.DOLocalPath(new Vector3[] { character.itemPlaceHand.transform.position, (otherCharacter.itemPlaceHand.transform.position + character.itemPlaceHand.transform.position) / 2 + new Vector3(0, stickyBombHeight, 0), otherCharacter.itemPlaceHand.transform.position }, stickyBombFlyTime, PathType.CubicBezier, PathMode.Full3D)
                .OnWaypointChange((int wayPointIndex) => {
                    if (wayPointIndex == 1)
                    {

                    }
                })
                .OnComplete(() => {
                    var bombBuff = new StickyBombBuff(otherCharacter);
                    bombBuff.SetExplosionTime(explosionTime, explosionMaxTime);
                    otherCharacter.OnGainBuff(bombBuff);
                    isPassingBomb = true;
                    base.Finish();
                });
        }
    }
}
