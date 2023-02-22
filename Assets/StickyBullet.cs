using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyBullet : MonoBehaviour
{
    public bool hasStick = false;
    FixedJoint fj;
    Rigidbody rb;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!hasStick && collision.rigidbody && collision.gameObject.GetComponent<CollisionStun>() != null)
        {
            hasStick = true;
            fj = collision.rigidbody.gameObject.AddComponent<FixedJoint>();
            fj.connectedBody = rb;
            fj.breakForce = 9000;
        }   
    }

    private void OnDestroy()
    {
        if (fj != null)
        {
            Destroy(fj);
        }
    }
}
