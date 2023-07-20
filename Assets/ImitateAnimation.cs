using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ImitateAnimation : MonoBehaviour
{
    public Transform targetTransform;
    public bool setPosition;
    public bool forceSet;
    Rigidbody rb;
    public float recoveryRate = 1f;
    public float currentTimer = 0;
    public float balanceRotationValue = 0;
    public float physicsRotationValue = 0;
    public BlendConstraint blendConstraint;
    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
        //SetAnimationControl();
    }

    private void FixedUpdate()
    {


    }

    private void OnCollisionEnter(Collision collision)
    {
        //SetPhysicsControl();
    }

    private void SetPhysicsControl()
    {
        blendConstraint.data.rotationWeight = physicsRotationValue;
        currentTimer = 0;
    }

    private void SetAnimationControl()
    {
        blendConstraint.data.rotationWeight = balanceRotationValue;
    }

    public void UpdatePosition()
    {
        if (setPosition)
        {
            this.transform.position = targetTransform.position;
            this.transform.rotation = targetTransform.rotation;
        }
        //rb.MoveRotation(targetTransform.rotation);
    }


    private void LateUpdate()
    {
        //UpdatePosition();
    }

    void Update()
    {
        //if (blendConstraint.data.rotationWeight != balanceRotationValue)
        //{
        //    currentTimer += Time.deltaTime;
        //    if (currentTimer >= recoveryRate)
        //    {
        //        currentTimer = 0;
        //        SetAnimationControl();
        //    }
        //}
        
    }
}
