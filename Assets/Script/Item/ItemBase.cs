using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemBase : MonoBehaviour
{
    [Header("µÀ¾ßµÄGameObject")]
    public GameObject itemGameObject;
    private float stayTime;
    private float currentTime;
    private bool isShow = false;
    public ItemBuffBase itemBuffBase;

    public virtual void CreatSkillItemm(float stayTimeData)
    {
        stayTime = stayTimeData;
        isShow = true;
        itemGameObject.SetActive(true);
        itemGameObject.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
            otherCollision.buffs.Add(CreatItemBuff());
        }
    }

    protected virtual ItemBuffBase CreatItemBuff()
    {
        return null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > stayTime)
        {
            OnEnd();
        }

    }

    public virtual void OnEnd()
    {
        DOTween.Kill(itemGameObject.transform);
        GameObject.Destroy(gameObject);
    }
}
