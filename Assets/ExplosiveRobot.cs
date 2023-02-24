using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveRobot : Weapon
{
    public float currentGas = 0f;
    public Rigidbody robotBody;
    private float deltaScale;
    public float maxScale;
    private float maxActorGas = 100;
    public float chargeTime;
    private bool releasing;
    private bool isGrounded;
    private bool canExplode = false;
    [Tooltip("µØÃæµÄLayers")]
    [SerializeField] LayerMask groundMask;
    public TriggerList triggerList;
    public GameObject explodeEffect;


    public override void Fire()
    {
        base.Fire();
        if (currentGas < maxActorGas && !releasing)
        {
            currentGas = currentGas + (maxActorGas - currentGas) / chargeTime * Time.fixedDeltaTime;
            releasing = false;
        }
    }

    public override void StopFire()
    {
        if (currentGas <= 50)
            return;
        canExplode = true;
        // Jump, gas reaches 0, explode the motherfucker.
        releasing = true;
        if (this.controller)
            this.controller.grab.Drop();

    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        if (currentGas <= 50)
            return;
        canExplode = true;
        // Jump, gas reaches 0, explode the motherfucker.
        releasing = true;
    }

    private void Awake()
    {
        deltaScale = (maxScale - 1) / maxActorGas;
    }

    private void FixedUpdate()
    {
        SetState();
        CheckIsGrounded();
        Move();
        Jump();
        UseGas();
    }

    private void CheckIsGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x, 0), 0.05f, groundMask);
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
            if (canExplode)
            {
                Explode();
                canExplode = false;
            }

        }
    }


    private void Explode()
    {
        var obj = GameObject.Instantiate(this.explodeEffect, new Vector3(this.transform.position.x, this.transform.position.y - 1, this.transform.position.z), Quaternion.identity);
        obj.SetActive(true);
        foreach(var body in triggerList.affectingBodies)
        {
            if (body != null)
            {
                var explodeDir = (body.position - this.robotBody.position).normalized;
                body.AddForce(explodeDir * 20f, ForceMode.Impulse);
            }
        }
    }

    private void Move()
    {
        if (currentGas > 0 && releasing)
        {
            this.robotBody.AddForce(this.transform.forward * 10f, ForceMode.Impulse);
        }
    }

    private void Jump()
    {
        if (currentGas > 0 && releasing && (isGrounded))
        {
            //this.robotBody.AddForce(Vector3.up * 200f, ForceMode.Impulse);
            //isGrounded = false;
        }
    }

    private void SetState()
    {
        robotBody.transform.localScale = new Vector3(currentGas * deltaScale + 1, currentGas * deltaScale + 1, currentGas * deltaScale + 1);
        robotBody.gameObject.GetComponent<ConfigurableJoint>().targetRotation = Quaternion.Slerp(robotBody.gameObject.GetComponent<ConfigurableJoint>().targetRotation, Quaternion.Euler(0, -this.transform.rotation.eulerAngles.y, 0), 0.1f);
    }
}
