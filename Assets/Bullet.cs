using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Weapon
{
    public float speed = 20f;
    public override void Fire()
    {
        controller.grab.Drop();
        this.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, speed));
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        var rigid = this.gameObject.AddComponent<Rigidbody>();
        rigid.mass = 2;
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        this.transform.localRotation = Quaternion.Euler(new Vector3(0, 90, 0));
        Destroy(rigidBody);
    }
}
