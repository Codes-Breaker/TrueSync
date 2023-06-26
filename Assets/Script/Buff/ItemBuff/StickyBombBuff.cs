using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StickyBombBuff : ItemBuffBase
{
    //��ը��Χ
    private float explosionRangeRadius = 10f;
    //��ը����С
    private float explosionForceArgument = 100f;
    //����ʱ�ķ��и߶�
    public float stickyBombHeight = 0.5f;
    //����ʱ�ķ���ʱ��
    public float stickyBombFlyTime = 0.1f;
    //��ը��ЧԤ����·��
    private string timeLapseBombExplosionEffectPath = "Prefabs/Effect/StickyBomb_Explosion";
    //����ʱ��Ч
    private string timeLapseBombCountDownEffectPath = "Prefabs/Effect/BombCountDown";
    //���ر�ը����ʱ
    private float explosionTime;
    //���ر�ը����ʱ��ʱ��
    public float explosionMaxTime;

    private bool isCreatCountDownEffect = false;

    private float currentFlyTime;

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
        currentFlyTime = 0f;
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
        if(isPassingBomb)
        {
            PassingBomb();
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

        //var colliders = GameObject.FindObjectsOfType<Rigidbody>();
        var colliders = Physics.OverlapSphere(stickyBombGameObject.transform.position, explosionRangeRadius);

        if (colliders.Length != 0)
        {
            foreach (var item in colliders)
            {
                if (item.GetComponent<Rigidbody>())
                {
                    if ((item.transform.position - stickyBombGameObject.transform.position).magnitude < explosionRangeRadius)
                    {
                        if (item.GetComponent<CharacterContorl>())
                            item.GetComponent<CharacterContorl>().AddExplosionForce(explosionForceArgument * item.GetComponent<Rigidbody>().mass, stickyBombGameObject.transform.position, explosionRangeRadius);
                        else
                            item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument * item.GetComponent<Rigidbody>().mass, stickyBombGameObject.transform.position, explosionRangeRadius);
                    }
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
            isPassingBomb = true;
        }
    }

    private void PassingBomb()
    {
        currentFlyTime += Time.deltaTime;
        float t = Mathf.Clamp01(currentFlyTime / stickyBombFlyTime);
        Vector3 curvePoint = CalculateParabolicPoint(stickyBombGameObject.transform.position, otherCharacter.transform.position, stickyBombHeight, t);
        stickyBombGameObject.transform.position = curvePoint;
        if (currentFlyTime > stickyBombFlyTime)
            OnComplete();
    }

    private void OnComplete()
    {
        var bombBuff = new StickyBombBuff(otherCharacter);
        bombBuff.SetExplosionTime(explosionTime, explosionMaxTime);
        otherCharacter.OnGainBuff(bombBuff);
        base.Finish();
    }

    private Vector3 CalculateParabolicPoint(Vector3 start, Vector3 end, float height, float t)
    {
        // ���������߹�ʽ���������ϵ�λ��
        float parabolicT = Mathf.Sin(t * Mathf.PI);
        Vector3 result = Vector3.Lerp(start, end, t);
        result.y += parabolicT * height;

        return result;
    }
}
