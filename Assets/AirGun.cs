using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirGun : Weapon
{
    public float currentGas = 0f;
    private float maxGas = 100;
    public float chargeTime;
    private bool releasing;
    private float deltaScale;
    public float maxScale;
    public Transform scalingObject;
    public float speed = 20f;
    public int timer = 100;
    private float originalTimer;
    public GameObject projectile;
    public Transform gun;
    public override void Fire()
    {
        base.Fire();
        if (currentGas < maxGas && !releasing)
        {
            currentGas = currentGas + (maxGas - currentGas) / chargeTime * Time.fixedDeltaTime;
            releasing = false;
        }
    }

    private void Awake()
    {
        deltaScale = (maxScale - 1) / maxGas;
        originalTimer = timer;
    }

    public override void StopFire()
    {
        if (currentGas <= 30)
            return;
        releasing = true;
    }

    private void SetState()
    {
        scalingObject.transform.localScale = new Vector3(currentGas * deltaScale + 1, currentGas * deltaScale + 1, currentGas * deltaScale + 1);
        gun.transform.localScale = new Vector3(0.2f, 0.2f + currentGas * (deltaScale / 2f), 0.2f);
    }

    private void UseGas()
    {
        if (!releasing)
            return;
        if (currentGas > 0)
        {
            currentGas--;
        }
        if (currentGas <= 0 && releasing)
        {
            releasing = false;
        }
    }

    private void Update()
    {
        originalTimer--;
    }

    private void FixedUpdate()
    {
        SetState();
        UseGas();
        originalTimer--;

        if (originalTimer < 0 && releasing)
        {
            GameObject instantiatedProjectile = GameObject.Instantiate(projectile, transform.position, transform.rotation);
            Physics.IgnoreCollision(instantiatedProjectile.GetComponent<Collider>(), GetComponent<Collider>());
            instantiatedProjectile.GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(0, 0, speed));
            originalTimer = timer;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        var rigid = this.gameObject.AddComponent<Rigidbody>();
        rigid.mass = weight;
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        var rigidBody = this.gameObject.GetComponent<Rigidbody>();
        Destroy(rigidBody);
        this.transform.localPosition = this.transform.localPosition - new Vector3(1, 0, 0);
        this.transform.localRotation = Quaternion.Euler(new Vector3(10, 90, 0));
    }

}
