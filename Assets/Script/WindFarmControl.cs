using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindFarmControl : MonoBehaviour,IRandomEventsObject
{
    List<Rigidbody> characterList;
    public float forceArgument;
    public float angleArgument;
    public float stayTime;
    private float currentTime;
    private bool isShow;

    public void OnShow(Vector3 point)
    {
        characterList = new List<Rigidbody>();
        this.gameObject.SetActive(true);
        isShow = true;
        var characterContorls = Object.FindObjectsOfType<CharacterContorl>();
        foreach(var item in characterContorls)
        {
            characterList.Add(item.GetComponent<Rigidbody>());
        }
    }

    public void OnExit()
    {
        isShow = false;
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
        float radians = angleArgument * Mathf.Deg2Rad;
        float x = Mathf.Cos(radians) ;
        float y = Mathf.Sin(radians) ;
        Vector3 forward = new Vector3(x, 0, y).normalized;

        if (isShow)
        {
            foreach(var rigid in characterList)
            {
                rigid.AddForce(forward * forceArgument, ForceMode.Force);
            }
        }
    }
}
