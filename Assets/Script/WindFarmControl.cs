using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class WindFarmControl : MonoBehaviour, IRandomEventsObject
{
    List<Rigidbody> characterList;
    public float forceArgument;
    public float angleArgument;
    private float stayTime;
    private float currentTime;
    private bool isShow;
    public float prepareShowTime;
    private bool startPrepare;
    private TMP_Text text;
    private List<int> randomAngle = new List<int>()
    {
        0,
        90,
        180,
        270,
    };
    private List<float> strengths = new List<float>()
    {
        20,
        30,
        35,
        40,
    };

    private void Awake()
    {
        text = GameObject.Find("Canvas/WindText").GetComponent<TMP_Text>();
    }
    public void OnShow(Vector3 point, float stayTime)
    {
        angleArgument = randomAngle[Random.Range(0, 4)];
        forceArgument = strengths[Random.Range(0, 4)];
        this.stayTime = stayTime;
        characterList = new List<Rigidbody>();
        this.gameObject.SetActive(true);
        isShow = false;
        startPrepare = true;
        var characterContorls = Object.FindObjectsOfType<CharacterContorl>();
        foreach (var item in characterContorls)
        {
            characterList.Add(item.GetComponent<Rigidbody>());
        }
    }

    public void OnExit()
    {
        isShow = false;
        startPrepare = false;
        text.text = "";
        gameObject.SetActive(false);
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
        float x = Mathf.Cos(radians);
        float y = Mathf.Sin(radians);
        Vector3 forward = new Vector3(x, 0, y).normalized;

        if (isShow)
        {
            text.text = $"wind speed: {forceArgument} \nwind direction: {angleArgument}\n{(Convert.ToInt32(stayTime - currentTime))}s";
            foreach (var rigid in characterList)
            {
                var character = rigid.GetComponent<CharacterContorl>();
                if (!character.invulernable && !character.returning)
                {
                    var vel1 = character.velocityBeforeCollision;
                    var d1 = Vector3.Angle(vel1, forward);

                    var degree1 = d1 * Mathf.Deg2Rad;
                    var m1 = (Mathf.Cos(degree1) * vel1).magnitude;
                    if (m1 <= character.maxReleaseVelocity)
                        rigid.AddForce(forward * forceArgument);
                }

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