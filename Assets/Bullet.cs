using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Weapon
{
    public float speed = 20f;
    public Vector3 velocityBeforeCollision = Vector3.zero;
    public Vector3 positionBeforeCollision = Vector3.zero;
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
    private void OnCollisionEnter(Collision collision)
    {
        if (body == null)
            return;
        if (collision.transform.gameObject.GetComponent<CollisionStun>())
        {
            Vector3 vel1 = velocityBeforeCollision;
            Vector3 cPoint = collision.contacts[0].point;
            Vector3 contactToOther = collision.transform.gameObject.GetComponent<CollisionStun>().positionBeforeCollision - cPoint;
            var d1 = Vector3.Angle(vel1, contactToOther);
            var degree1 = d1 * Mathf.Deg2Rad;
            var m1 = (Mathf.Cos(degree1) * vel1).magnitude * body.GetComponent<Rigidbody>().mass;
            if (collision.collider.GetComponent<CharacterManager>().isSwimmy)
                return;
            if (m1 > 70)
            {
                collision.collider.GetComponent<CharacterManager>().currentHPValue = collision.collider.GetComponent<CharacterManager>().currentHPValue - 10;
            }
            else if (m1 > 50)
            {
                collision.collider.GetComponent<CharacterManager>().currentHPValue = collision.collider.GetComponent<CharacterManager>().currentHPValue - 8;
            }
            else if (m1 > 30)
            {
                collision.collider.GetComponent<CharacterManager>().currentHPValue = collision.collider.GetComponent<CharacterManager>().currentHPValue - 6;
            }
            else if (m1 > 15)
            {
                collision.collider.GetComponent<CharacterManager>().currentHPValue = collision.collider.GetComponent<CharacterManager>().currentHPValue - 4;
            }
        }
    }

    private void FixedUpdate()
    {
        if (body != null)
        {
            Vector3 vel1 = body.velocity;
            velocityBeforeCollision = vel1;
            positionBeforeCollision = body.position;
        }

    }
}
