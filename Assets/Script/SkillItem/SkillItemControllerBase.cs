using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillItemControllerBase : MonoBehaviour
{
    public CharacterContorl user;
    public Image iconPrefab;
    private Image icon;
    private float stayTime;
    private float currentTime;
    public GameObject itemGameObject;
    private bool isShow = false;

    public virtual void CreatSkillItemm(float stayTimeData)
    {
        stayTime = stayTimeData;
        isShow = true;
        itemGameObject.SetActive(true);
        itemGameObject.transform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    public virtual void UseSkillItem()
    {
        user.isUseSkill = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<CharacterContorl>())
        {
            var otherCollision = collision.gameObject.GetComponent<CharacterContorl>();
            if (otherCollision.skill == null && user == null)
            {
                user = otherCollision;
                user.skill = this;
                transform.GetComponent<MeshRenderer>().enabled = false;
                Init();
            }
        }
    }
    protected virtual void Init()
    {
        itemGameObject.SetActive(false);
        CreatIcon();
    }

    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > stayTime && user == null)
        {
            OnEnd();
        }
        
    }

    private void LateUpdate()
    {
        if (user)
            SetIcon();
    }

    private void CreatIcon()
    {
        icon = Instantiate(iconPrefab, user.canvas.transform);
    }

    private void SetIcon()
    {
        user.canvas.transform.forward = Camera.main.transform.forward;
        icon.transform.position = user.transform.position;
        icon.transform.localPosition = icon.transform.localPosition + new Vector3(0, 2.1f + (user.bodyCollider.transform.localScale.x - 1) * 1.2f, 0);
    }

    public virtual void ExitUseMode()
    {
        user.isUseSkill = false;
        DestoryIcon();
    }

    private void DestoryIcon()
    {
        if(icon)
            GameObject.Destroy(icon);
    }

    protected virtual void OnEnd()
    {
        if(user)
        {
            user.isUseSkill = false;
            user = null;
        }
        DestoryIcon();
        GameObject.Destroy(gameObject);
    }

}