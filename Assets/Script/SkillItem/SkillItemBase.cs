using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ʱҪ��������
public struct SkillItemCreatData
{
    public Vector3 targetPosition;
}

public class SkillItemBase : MonoBehaviour
{

    protected SkillItemCreatData skillItemData;
    public virtual void Init(SkillItemCreatData skillItemCreatData)
    {
        skillItemData = skillItemCreatData;
    }

    public virtual void Show()
    {

    }

    public virtual void End()
    {

    }
}
