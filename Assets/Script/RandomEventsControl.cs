using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RandomEvent
{
    public string randomEventsPath;
    public float startTime;
    public Vector3 startPosition;
    public float stayTime;
}

public class RandomEventsControl : MonoBehaviour
{
    string lglooPath = "Prefabs/Lgloo";
    string submarinePath = "Prefabs/Submarine";
    string winFarmPath = "Prefabs/WindFarm";

    private float currrentTime;

    List<RandomEvent> randomEvents = new List<RandomEvent>();
    List<RandomEvent> removeRandomEvents = new List<RandomEvent>();
    private void Start()
    {
        currrentTime = 0;

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 0,
            randomEventsPath = submarinePath,
            stayTime = 20,
        }) ;

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 0,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 120,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 180,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 60,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });
        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 150,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });
        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 90,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0.08f, 0, 0.75f),
            startTime = 210,
            randomEventsPath = lglooPath,
            stayTime = 20,
        });
        
        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 0,
            randomEventsPath = winFarmPath,
            stayTime = 20,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 40,
            randomEventsPath = winFarmPath,
            stayTime = 40,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 20,
            randomEventsPath = submarinePath,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 80,
            randomEventsPath = submarinePath,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 90,
            randomEventsPath = winFarmPath,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 150,
            randomEventsPath = winFarmPath,
        });

        randomEvents.Add(new RandomEvent
        {
            startPosition = new Vector3(0, 0, 0),
            startTime = 200,
            randomEventsPath = winFarmPath,
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
    }
}
