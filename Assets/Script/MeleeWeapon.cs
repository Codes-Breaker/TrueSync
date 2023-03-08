using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [HideInInspector]
    public CharacterManager controller;
    public Rigidbody body;
    public Vector3 offsetRotation;
    public Vector3 offsetPosition;
    private float mass;
    public Vector3 targetRotationL;
    public Vector3 targetRotationR;

    private void Awake()
    {
        body = this.GetComponent<Rigidbody>();
        mass = body.mass;
    }
    public virtual void OnEquipped(CharacterManager controller)
    {
        this.controller = controller;
        //Destroy(body);
    }

    public virtual void OnUnEquipped()
    {
        this.controller = null;
        //body = this.gameObject.AddComponent<Rigidbody>();
        //body.mass = mass;
    }

    public virtual void OnCharge()
    {

    }

    public virtual void OnRelease()
    {
    }
}
