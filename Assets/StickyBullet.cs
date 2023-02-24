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
        if (collision.rigidbody && collision.gameObject.GetComponent<Rigidbody>() != null && (collision.gameObject.CompareTag("Player")))
        {
            fj = collision.rigidbody.gameObject.AddComponent<FixedJoint>();
            fj.connectedBody = rb;
            fj.breakForce = 5000;
            this.transform.localScale = new Vector3(2, 2, 2);
            hasStick = true;
        }   
    }

    private void FixedUpdate()
    {
        if (hasStick)
        {
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
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
