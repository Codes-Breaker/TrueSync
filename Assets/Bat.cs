using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Weapon
{
    public Transform offset;
    public float speed = 20f;
    public int timer = 2000;
    private float originalTimer;
    public override void Fire()
    {
        if (originalTimer < 0)
        {
            //controller.swinging = true;
            base.Fire();
            this.body.AddTorque(new Vector3(250, 0, 0), ForceMode.Impulse);
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
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        controller.swinging = false;
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        this.transform.position = offset.transform.position;
    }

}
