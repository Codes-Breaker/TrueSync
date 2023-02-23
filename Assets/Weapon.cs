using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterManager controller;
    public FixedJoint fixJoint;
    public bool canGrabInMouth = false; //ÄÜ±»×ìµð×Å
    protected Rigidbody body;
    public float weight = 1;
    private void Awake()
    {
        body = this.GetComponent<Rigidbody>();
    }

    public virtual void OnEquipped()
    {

    }

    public virtual void OnUnEquipped()
    {

    }

    public virtual void Fire()
    {

    }

    public virtual void StopFire()
    {

    }
}
