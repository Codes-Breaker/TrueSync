using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RandomEvent
{
    public GameObject randomEventsObject;
    public float startTime;
    public Vector3 startPosition;
}

public class RandomEventsControl : MonoBehaviour
{
    public GameObject lglooObject;
    public GameObject submarineObjct;
    public GameObject windFarmObject;

    private float currrentTime;

    List<RandomEvent> randomEvents = new List<RandomEvent>();
    List<RandomEvent> removeRandomEvents = new List<RandomEvent>();
    private void Start()
    {
        currrentTime = 0;

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 2.07f),
            startTime = 30,
            randomEventsObject = submarineObjct,
        }) ;

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(-0.53f, 0, -3.36f),
            startTime = 10,
            randomEventsObject = lglooObject,
        });
        
        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 10,
            randomEventsObject = windFarmObject,
        });

    }

    // Update is called once per frame
    void Update()
    {
        currrentTime += Time.deltaTime;
        foreach(var item in randomEvents)
        {
            if(currrentTime > item.startTime)
            {
                item.randomEventsObject.GetComponent<IRandomEventsObject>().OnShow(item.startPosition);
                removeRandomEvents.Add(item);
            }
        }
        foreach(var item in removeRandomEvents)
        {
            randomEvents.Remove(item);
        }
        removeRandomEvents.Clear();
    }
}
