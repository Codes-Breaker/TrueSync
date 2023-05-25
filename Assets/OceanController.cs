using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OceanController : MonoBehaviour
{
    public float defaultSeaLevel = 0f;

    public float maxSeaLevel = 2f;

    public float riseTime = 20;

    private bool startRising = false;

    public void StartRising()
    {
        if (!startRising)
        {
            startRising = true;
            this.transform.DOLocalMoveY(maxSeaLevel, riseTime);
        }  
    }

    // Start is called before the first frame update
    void Start()
    {
        startRising = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
