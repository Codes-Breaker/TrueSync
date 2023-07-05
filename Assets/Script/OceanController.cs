using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Crest;

public class OceanController : MonoBehaviour
{
    public float defaultSeaLevel = 0f;

    public float maxSeaLevel = 2f;

    public float riseTime = 20;

    private bool startRising = false;
    public OceanDepthCache oceanCache;
    public void StartRising()
    {
        if (!startRising)
        {
            startRising = true;
            this.transform.DOLocalMoveY(maxSeaLevel, riseTime);
        }  
    }

    public void Rise(float seaLevel, float riseTime)
    {
        this.transform.DOLocalMoveY(seaLevel, riseTime).onComplete += () =>
        {
            oceanCache.PopulateCache(true);
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        startRising = false;
        oceanCache.PopulateCache(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
