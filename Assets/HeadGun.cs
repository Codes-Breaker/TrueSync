using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadGun : Weapon
{
    public GameObject projectile;
    public float speed = 20f;
    public int timer = 2000;
    private float originalTimer;

    private void Awake()
    {
        originalTimer = timer;
    }

    private void Update()
    {
        originalTimer--;
    }

    public override void Fire()
    {
        if (originalTimer < 0)
        {
            GameObject instantiatedProjectile = GameObject.Instantiate(projectile, transform.position +  transform.forward.normalized * 0.1f, transform.rotation);
            instantiatedProjectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, speed));
            originalTimer = timer;
        }
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        var rigid = this.gameObject.AddComponent<Rigidbody>();
        rigid.mass = 1;
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        Destroy(rigidBody);
        this.transform.localPosition = this.transform.localPosition + new Vector3(0, 0, 0);
        this.transform.localRotation = Quaternion.Euler(new Vector3(10, 90, 0));
    }
}
