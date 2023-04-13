using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillItemControllerBase : MonoBehaviour
{
    public CharacterContorl user;
    public Image iconPrefab;
    private Image icon;
    
    public void CreatSkillItemm()
    {

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
        CreatIcon();
    }

    private void Update()
    {
        if(user)
            SetIcon();
    }

    private void CreatIcon()
    {
        icon = Instantiate(iconPrefab, user.canvas.transform);
    }

    private void SetIcon()
    {
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
        GameObject.Destroy(icon);
    }

    protected virtual void OnEnd()
    {
        user.isUseSkill = false;
        user = null;
        DestoryIcon();
        GameObject.Destroy(gameObject);
    }

}