using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCreator : MonoBehaviour
{
    public List<GameObject> itemList;
    public float Time = 2000;

    private float originalTimer = 0;

    private void Awake()
    {
        originalTimer = Time;
        Random.InitState(System.DateTime.Now.Millisecond);
    }

    public void Update()
    {
        Time--;
        if (Time <= 0)
        {
            Time = originalTimer;
            GameObject instantiatedProjectile = GameObject.Instantiate(itemList[Random.Range(0, itemList.Count)], transform.position, transform.rotation);
        }
    }

}
