using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeHammerWeapon : MeleeWeapon
{
    SkinnedMeshRenderer renderer;
    public float gas = 0f;
    public float maxGas = 100.0f;
    public float maxHeight = 5f;
    private Vector3 originalRotationL;
    private Vector3 originalRotationR;
    public Vector3 maxRotationL;
    public Vector3 maxRotationR;
    public Vector3 tempHitRotation = new Vector3(-180, 0, 0);
    public bool release;
    public List<CharacterManager> actors = new List<CharacterManager>();
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SkinnedMeshRenderer>();
        originalRotationL = targetRotationL;
        originalRotationR = targetRotationR;
    }

    public override void OnCharge()
    {
        base.OnCharge();
        gas = Mathf.Min(gas + 1, maxGas);
        var gasPer = gas / maxGas;
        targetRotationL = Vector3.Lerp(originalRotationL, maxRotationL, gasPer);
        targetRotationR = Vector3.Lerp(originalRotationR, maxRotationR, gasPer);
    }

    public override void OnRelease()
    {
        base.OnRelease();
        release = true;
    }

    private void FixedUpdate()
    {
        if (controller != null)
        {
            var canFly = Physics.CheckSphere(controller.bodyCollider.transform.position - new Vector3(0, (controller.bodyCollider as SphereCollider).radius / 2, 0), maxHeight, controller.groundMask);
            if (canFly)
            {
                var rb = this.controller.ridbody;
                var m_fanForce = 200f * (gas / maxGas);
                var m_damping = 0;
                Vector3 velocity = rb.velocity;
                velocity += Vector3.up * m_fanForce * Time.deltaTime; // dir = fan direction, ie. transform.up or whatever setup you have there
                velocity -= velocity * m_damping * Time.deltaTime; // add dampening so that velocity doesn't get out of hand
                rb.velocity = velocity;
            }

        }

        //if (release)
        //{
        //    this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(tempHitRotation), 5f);
        //    if (this.transform.localRotation == Quaternion.Euler(tempHitRotation))
        //    {
        //        //Explode
        //        Explode();
        //        this.controller.DropWeapon();
        //    }
        //}


    }

    public void Explode()
    {
        foreach(var actor in actors)
        {
            actor.collisionStun.fall = true;
            actor.collisionStun.maxFallTime = 5;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterManager>() != null)
        {
            actors.Add(other.gameObject.GetComponent<CharacterManager>());
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CharacterManager>() != null)
        {
            actors.Remove(other.gameObject.GetComponent<CharacterManager>());
        }
    }

    public override void OnUnEquipped()
    {
        base.OnUnEquipped();
        release = false;
        this.body = this.gameObject.AddComponent<Rigidbody>();
    }

    public override void OnEquipped(CharacterManager controller)
    {
        base.OnEquipped(controller);
        Destroy(this.body);
    }

    // Update is called once per frame
    void Update()
    {
        renderer.SetBlendShapeWeight(0, gas);

        
    }
}
