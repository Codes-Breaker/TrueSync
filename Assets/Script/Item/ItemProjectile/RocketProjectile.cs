using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class RocketProjectile : ItemProjectileBase
{
    private Vector3 initialVelocity = Vector3.zero;
    public Quaternion itemRot = Quaternion.identity;
    public Vector3 rocketForward = Vector3.zero;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        rb.velocity = initialVelocity;
    }

    public override void Init(CharacterContorl character)
    {
        base.Init(character);
    }

    public override void Launch()
    {

    }

    public override void Throw()
    {
        this.transform.parent = character.itemPlaceBelly;
        this.transform.localPosition = Vector3.zero;
        this.transform.rotation = itemRot;
        this.transform.parent = null;
        this.project = rocketForward;
        Physics.IgnoreCollision(this.bodyCollider, character.bodyCollider, true);
        rb.velocity = (character.ridbody.velocity.magnitude) * project;
        initialVelocity = rb.velocity;
    }
}

