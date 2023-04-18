using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct RandomEvent
{
    public string randomEventsPath;
    public float startTime;
    public Vector3 startPosition;
    public float stayTime;
}
public struct SkillItem
{
    public float startTime;
    public Vector3 startPosition;
    public string skillItemPath;
    public float stayTime;
}

public class RandomEventsControl : MonoBehaviour
{
    string lglooPath = "Prefabs/Lgloo1";
    string submarinePath = "Prefabs/Submarine";
    string winFarmPath = "Prefabs/WindFarm";
    string missileBoomPath = "Prefabs/MissileBoomController";



    public int lglooGenerateFrequency;
    public int lglooStayTime;
    [Range(0, 1)]
    public float lglooRollRate;
    public int lglooStartDelay;

    public int submarineGenerateFrequency;
    public int submarineStayTime;
    [Range(0, 1)]
    public float submarineRollRate;
    public int submarineStartDelay;

    public int winFarmGenerateFrequency;
    public int winFarmStayTime;
    [Range(0, 1)]
    public float winFarmRollRate;
    public int winFarmStartDelay;

    public int missileBoomGenerateFrequency;
    [Range(0, 1)]
    public float missileBoomRollRate;
    public int missileBoomStartDelay;
    public int missileBoomStayTime;

    private float currrentTime;

    List<RandomEvent> randomEvents = new List<RandomEvent>();
    List<RandomEvent> removeRandomEvents = new List<RandomEvent>();

    List<SkillItem> skillItems = new List<SkillItem>();
    List<SkillItem> removeSkillItems = new List<SkillItem>();
    private void RandomGenerate()
    {
        //5分钟时间随机
        var totalTime = 5 * 60;

        //导弹生成随机生成算法
        var missileBooms = Convert.ToInt32(totalTime / missileBoomGenerateFrequency);
        for (int i = 0; i <= missileBooms; i++)
        {
            float weight = Random.Range(0f, 1f);
            if (weight <= missileBoomRollRate)
            {
                skillItems.Add(new SkillItem
                {
                    startPosition = new Vector3(0, 2, -5.44f),
                    startTime = (i) * missileBoomGenerateFrequency + missileBoomStartDelay,
                    skillItemPath = missileBoomPath,
                    stayTime = missileBoomStayTime,
                });
            }
        }

        //雪屋随机生成算法
        var lglooRolls = Convert.ToInt32(totalTime / lglooGenerateFrequency);
        for (int i = 0; i <= lglooRolls; i++)
        {
            float weight = Random.Range(0f, 1f);
            if (weight <= lglooRollRate)
            {
                randomEvents.Add(new RandomEvent
                {
                    startPosition = new Vector3(0.08f, 0, 0.75f),
                    startTime = (i) * lglooGenerateFrequency + lglooStartDelay,
                    randomEventsPath = lglooPath,
                    stayTime = lglooStayTime,
                });
            }
        }
        //潜水艇随机生成算法
        var submarineRolls = Convert.ToInt32(totalTime / submarineGenerateFrequency);
        for (int i = 0; i <= submarineRolls; i++)
        {
            float weight = Random.Range(0f, 1f);
            if (weight <= submarineRollRate)
            {
                randomEvents.Add(new RandomEvent
                {
                    startPosition = new Vector3(0.08f, 0, 0.75f),
                    startTime = (i) * submarineGenerateFrequency + submarineStartDelay,
                    randomEventsPath = submarinePath,
                    stayTime = submarineStayTime,
                });
            }
        }

        //风场随机生成算法
        var winFarmRolls = Convert.ToInt32(totalTime / winFarmGenerateFrequency);
        for (int i = 0; i <= winFarmRolls; i++)
        {
            float weight = Random.Range(0f, 1f);
            if (weight <= winFarmRolls)
            {
                randomEvents.Add(new RandomEvent
                {
                    startPosition = new Vector3(0.08f, 0, 0.75f),
                    startTime = (i) * winFarmGenerateFrequency + winFarmStartDelay,
                    randomEventsPath = winFarmPath,
                    stayTime = winFarmStayTime,
                });
            }
        }
    }

    private void Start()
    {
        currrentTime = 0;
        RandomGenerate();
    }

    // Update is called once per frame
    void Update()
    {
        currrentTime += Time.deltaTime;
        foreach(var item in randomEvents)
        {
            if(currrentTime > item.startTime)
            {
                var eventObjectPrefab = Resources.Load<GameObject>(item.randomEventsPath);
                var eventObjectGameObject = Instantiate(eventObjectPrefab,item.startPosition,Quaternion.Euler(new Vector3(0,0,0)));
                eventObjectGameObject.GetComponent<IRandomEventsObject>().OnShow(item.startPosition,item.stayTime);
                removeRandomEvents.Add(item);
            }
        }
        foreach(var item in removeRandomEvents)
        {
            randomEvents.Remove(item);
        }
        removeRandomEvents.Clear();

        foreach (var item in skillItems)
        {
            if (currrentTime > item.startTime)
            {
                var objectPrefab = Resources.Load<GameObject>(item.skillItemPath);
                var objectGameObject = Instantiate(objectPrefab, item.startPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
                objectGameObject.SetActive(true);
                objectGameObject.GetComponent<SkillItemControllerBase>()?.CreatSkillItemm(item.stayTime);
                removeSkillItems.Add(item);
            }
        }
        foreach (var item in removeSkillItems)
        {
            skillItems.Remove(item);
        }
        removeSkillItems.Clear();
    }
}
