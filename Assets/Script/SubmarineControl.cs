using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmarineControl : MonoBehaviour,IRandomEventsObject
{
    public float stayTime;
    private float currentTime;
    private bool isShow;
    private float startTime;
    public float endTime;

    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if(currentTime > stayTime)
        {
            OnExit();
        }
    }

    public void OnExit()
    {
        transform.DOLocalMoveY(0f, endTime).OnComplete(() => {
            gameObject.SetActive(false);
        });
    }

    public void OnShow(Vector3 position,float stayTime)
    {
        this.stayTime = stayTime;
        transform.position = position;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        currentTime = 0;
        this.gameObject.SetActive(true);
        transform.DOLocalMoveY(2.3f, startTime).OnComplete(() => {
            isShow = true;
        });
    }
}
