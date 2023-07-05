using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IceTerrainController : MonoBehaviour
{
    [Header("掉落频率")]
    public float dropFrequency;
    [Header("一共掉落次数")]
    public int dropTimes;
    [Header("一次掉多少块")]
    public int singleDropNum;
    public GameController gameController;
    public List<IceTerrainDropController> totalPlanes = new List<IceTerrainDropController>();
    private float nextDropTimes;
    private int currentDropTimes = 0;

    private void Start()
    {
        nextDropTimes = dropFrequency;
    }
    // Start is called before the first frame update
    private void FixedUpdate()
    {
        if (gameController.startGame && currentDropTimes < dropTimes)
        {
            if (gameController.gameTime >= nextDropTimes)
            {
                nextDropTimes += dropFrequency;
                WarmDrop();
            }
        }
    }

    private void WarmDrop()
    {
        currentDropTimes++;
        for (int i = 0; i < singleDropNum; i++)
        {
            var randomIndex = Random.Range(0, totalPlanes.Count);
            var dropPlane = totalPlanes[randomIndex];
            dropPlane.PrepareDrop();
            totalPlanes.RemoveAt(randomIndex);
        }
    }
}
