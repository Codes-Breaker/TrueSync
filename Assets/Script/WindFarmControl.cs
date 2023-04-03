using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class WindFarmControl : MonoBehaviour,IRandomEventsObject
{
    List<Rigidbody> characterList;
    public float forceArgument;
    public float angleArgument;
    public float stayTime;
    private float currentTime;
    private bool isShow;
    public float prepareShowTime;
    private bool startPrepare;
    public TMP_Text text;
    private List<int> randomAngle = new List<int>()
    {
        0,
        90,
        180,
        270,
    };
    private List<float> strengths = new List<float>()
    {
        45,
        46,
        47,
        48,
    };
    public void OnShow(Vector3 point)
    {
        angleArgument = randomAngle[Random.Range(0, 4)];
        forceArgument = strengths[Random.Range(0, 4)];
        characterList = new List<Rigidbody>();
        this.gameObject.SetActive(true);
        isShow = false;
        startPrepare = true;
        var characterContorls = Object.FindObjectsOfType<CharacterContorl>();
        foreach(var item in characterContorls)
        {
            characterList.Add(item.GetComponent<Rigidbody>());
        }
    }

    public void OnExit()
    {
        isShow = false;
        startPrepare = false;
        text.text = "";
    }

    private void Update()
    {
        if (!isShow)
            return;
        currentTime += Time.deltaTime;
        if (currentTime > stayTime)
        {
            OnExit();
        }
    }



    private void FixedUpdate()
    {
        if (startPrepare)
        {
            prepareShowTime = prepareShowTime -= Time.fixedDeltaTime;
            if (prepareShowTime <= 0)
            {
                isShow = true;
            }
        }

        float radians = angleArgument * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) ;
        float y = Mathf.Sin(radians) ;
        Vector3 forward = new Vector3(x, 0, y).normalized;

        if (isShow)
        {
            text.text = $"wind speed: {forceArgument} \nwind direction: {angleArgument}\n{(Convert.ToInt32(stayTime - currentTime))}s";
            foreach (var rigid in characterList)
            {
                if (!rigid.GetComponent<CharacterContorl>().invulernable)
                    rigid.AddForce(forward * forceArgument);
            }
        }
        else if (startPrepare)
        {
            text.text = $"prepare wind {Convert.ToInt32(prepareShowTime)}s";
        }
        else
        {
            text.text = "";
        }
    }
}
