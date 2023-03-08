using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    public Transform targetLimb;
    ConfigurableJoint cj;
    Vector3 startRot;
    // Start is called before the first frame update
    void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
        startRot = targetLimb.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        cj.targetRotation =  Quaternion.Euler(startRot - targetLimb.localEulerAngles);
    }
}
