using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Weapon
{
    public Transform offset;
    public float speed = 20f;
    public int timer = 2000;
    private float originalTimer;
    public BatHit hit;

    private void Awake()
    {
        hit.gameObject.SetActive(false);
    }
    public override void Fire()
    {
        if (originalTimer < 0)
        {
            hit.gameObject.SetActive(true);
            controller.swinging = true;
            base.Fire();
            //this.body.AddTorque(new Vector3(250, 0, 0), ForceMode.Impulse);
            originalTimer = timer;
        }

    }

    private void Update()
    {
        originalTimer--;
    }

    public override void StopFire()
    {
        base.StopFire();
        controller.swinging = false;
        hit.gameObject.SetActive(false);
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        hit.gameObject.SetActive(false);
        controller.swinging = false;
        var rigid = this.gameObject.AddComponent<Rigidbody>();
        rigid.mass = 5;
        //this.gameObject.layer = 0;
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        Destroy(rigidBody);
        this.transform.localPosition = this.transform.localPosition + new Vector3(1f, 0, 0);
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        //this.gameObject.layer = this.controller.gameObject.layer;
    }

}
