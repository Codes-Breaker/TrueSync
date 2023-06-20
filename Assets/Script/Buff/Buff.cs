using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public CharacterContorl character;
    public bool isEnd = false;
    public float buffTime = -1;
    public float maxBuffTime = -1;
    public Buff(CharacterContorl target)
    {
        this.character = target;
        OnBuffApply();
    }

    public Buff(CharacterContorl target,float buffTime)
    {
        this.character = target;
        this.buffTime = buffTime;
        this.maxBuffTime = buffTime;
        OnBuffApply();
    }

    public virtual void OnBuffApply()
    {

    }

    public virtual void OnBuffRemove()
    {

    }

    public virtual void OnBuffUpdate()
    {
        if (buffTime != -1)
        {
            buffTime = buffTime - Time.deltaTime;
            if (buffTime <= 0)
            {
                Finish();
            }
        }
    }

    public virtual void OnBuffLateUpdate()
    {

    }

    public virtual void OnCollide(Collision collision)
    {

    }

    public void Finish()
    {
        OnBuffRemove();
        isEnd = true;
    }

}
