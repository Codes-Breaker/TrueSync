using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
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
            GameObject instantiatedProjectile = GameObject.Instantiate(projectile, transform.position, transform.rotation);
            instantiatedProjectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, speed));
            originalTimer = timer;
        }
    }
}
