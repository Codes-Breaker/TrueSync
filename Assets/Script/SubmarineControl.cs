using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SubmarineControl : MonoBehaviour,IRandomEventsObject
{
    public float stayTime;
    private float currentTime;
    private bool isShow;


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
        transform.DOLocalMoveY(0f, 10f).OnComplete(() => {
            this.gameObject.SetActive(false);
        });
    }

    public void OnShow(Vector3 position)
    {
        transform.position = position;
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        currentTime = 0;
        this.gameObject.SetActive(true);
        transform.DOLocalMoveY(2.3f, 10f).OnComplete(() => {
            isShow = true;
        });
    }
}
