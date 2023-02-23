using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb)
        {
            var dir = (other.gameObject.transform.position - this.gameObject.transform.position).normalized;
            rb.AddForce(dir * 30f * Mathf.Max(rb.mass, 2), ForceMode.Impulse);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(this.transform.position,  -this.transform.forward));
    }

}
