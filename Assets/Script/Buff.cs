using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public CharacterContorl character;
    public bool isEnd = false;
    public float buffTimes = -1;
    public Buff(CharacterContorl target)
    {
        this.character = target;
    }

    public virtual void OnBuffApply()
    {

    }

    public virtual void OnBuffRemove()
    {

    }

    public virtual void OnBuffUpdate()
    {
        if (buffTimes != -1)
        {
            buffTimes = buffTimes - Time.fixedDeltaTime;
            if (buffTimes <= 0)
            {
                isEnd = true;
            }
        }

    }

    public virtual void OnCollide(Collision collision)
    {

    }

    public void Finish()
    {
        isEnd = true;
    }

}
