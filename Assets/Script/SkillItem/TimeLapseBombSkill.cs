using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TimeLapseBombSkill: SkillItemBase
{
    private string timeLapseBombExplosionEffectPath = "Prefabs/Effect/StickyBomb_Explosion";
    private string timeLapseBombCountDownEffectPath = "Prefabs/Effect/BombCountDown";
    [Header("±¬Õ¨ÑÓÊ±")]
    public float explosionDelayTime;
    [Header("±¬Õ¨·¶Î§")]
    public float explosionRangeRadius;
    [Header("±¬Õ¨Á¦")]
    public float explosionForceArgument;
    [Header("µ¼µ¯·ÉÐÐ¸ß¶È")]
    public float BoomHeight = 2f;

    public Transform root;

    private bool isCountDown;
    private bool isAddCountDownEffct;
    private float currentTime;

    public override void Init(SkillItemCreatData skillItemCreatData)
    {
        base.Init(skillItemCreatData);
        isCountDown = false;
        isAddCountDownEffct = false;
    }
    public override void Show()
    {
        base.Show();
        LaunchTimeLapseBomb();
    }

    private void Update()
    {
        if(isCountDown)
        {
            currentTime += Time.deltaTime;
            if (currentTime > explosionDelayTime)
            {
                TimeLapseBombExplode();
            }
            else
                root.localScale = new Vector3(1,1-currentTime/explosionDelayTime,1);
            if (currentTime > explosionDelayTime * 0.6f && !isAddCountDownEffct)
                AddCountDownEffct();
        }
    }

    public void LaunchTimeLapseBomb()
    {
        //transform.GetComponent<Rigidbody>().isKinematic = true;
        transform.DOPath(new Vector3[] { transform.position, (transform.position + skillItemData.targetPosition) / 2 + Vector3.up * BoomHeight, skillItemData.targetPosition + Vector3.up * 2 }, 2f, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                TimeLapseBombBommbCoutDown();
            });
    }

    public void AddCountDownEffct()
    {
        isAddCountDownEffct = true;
        GameObject countDownEffctPrefab = (GameObject)Resources.Load(timeLapseBombCountDownEffectPath);
        GameObject countDownEffctGameObject = Instantiate(countDownEffctPrefab, transform);
        countDownEffctGameObject.transform.localPosition = Vector3.zero;
        countDownEffctGameObject.SetActive(true);

    }
    public void TimeLapseBombBommbCoutDown()
    {
        DOTween.Kill(transform);
        isCountDown = true;
        //transform.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void TimeLapseBombExplode()
    {
        var colliders = FindObjectsOfType<CharacterContorl>();

        if(colliders.Length !=0 )
        {
            foreach(var item in colliders)
            {
                if ((item.transform.position - transform.position).magnitude < explosionRangeRadius)
                {
                    if(!item.invulernable)
                        item.GetComponent<Rigidbody>().AddExplosionForce(explosionForceArgument, skillItemData.targetPosition, explosionRangeRadius);
                }
            }

        }
        var effectPrefab = Resources.Load(timeLapseBombExplosionEffectPath);
        var effectGameObject = (GameObject)Instantiate(effectPrefab, transform.position, Quaternion.Euler(Vector3.zero));
        Destroy(effectGameObject, 3f);
        Destroy(gameObject);


    }
}
