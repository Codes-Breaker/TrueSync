using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Crest;

public class IceTerrainDropController : MonoBehaviour
{
    [Header("预警时间")]
    public float warnTime = 10f;
    [Header("掉落时间")]
    public float dropTime = 5f;
    private bool preparingDrop = false;
    private bool startDroping = false;
    private float currentDropTime = 0f;
    public void PrepareDrop()
    {
        if (preparingDrop)
            return;
        preparingDrop = true;
        this.transform.DOScale(new Vector3(0.98f, 0.98f, 0.98f), warnTime).onComplete += () =>
        {
            this.transform.DOLocalMoveY(-10, dropTime);
            startDroping = true;
        };
        //this.transform.DOShakePosition(warnTime, new Vector3(0, 1, 0), 20, 90f, false, false).onComplete += () =>
        // {
        //     this.transform.DOLocalMoveY(-10, dropTime);
        //     startDroping = true;
        // };
    }

    private void FixedUpdate()
    {
        if (startDroping)
        {
            currentDropTime += Time.fixedDeltaTime;
            if (currentDropTime >= dropTime)
            {
                startDroping = false;
                GameObject.Destroy(this);
            }
        }
    }

}
