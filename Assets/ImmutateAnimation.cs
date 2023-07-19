using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmutateAnimation : MonoBehaviour
{
    public Transform targetTransform;
    public bool setPosition;
    public bool forceSet;
    Rigidbody rb;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {


    }

    public void UpdatePosition()
    {
        rb.MoveRotation(targetTransform.rotation);
        if (setPosition)
        {
            if (forceSet)
                this.transform.position = targetTransform.position;
            else
                this.transform.position = Vector3.Lerp(this.transform.position, targetTransform.position, 0.1f);
            rb.MovePosition(targetTransform.position);
        }

    }


    private void LateUpdate()
    {

    }

    void Update()
    {
        //rb.MoveRotation(targetTransform.rotation);
        //if (setPosition)
        //    rb.MovePosition(targetTransform.position);
    }
}
