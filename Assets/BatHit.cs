using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatHit : MonoBehaviour
{
    public Rigidbody ignoreRb;
    private void OnTriggerEnter(Collider other)
    {
        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb && ignoreRb != rb)
        {
            var dir = (other.gameObject.transform.position - this.gameObject.transform.position).normalized;
            rb.AddForce(dir * 30f * Mathf.Max(rb.mass, 2), ForceMode.Impulse);
            if (other.GetComponent<CharacterManager>())
            {
                other.GetComponent<CharacterManager>().currentHPValue = other.GetComponent<CharacterManager>().currentHPValue - 12;
                if (other.GetComponent<CollisionStun>())
                {
                    other.GetComponent<CollisionStun>().fall = true;
                    other.GetComponent<CollisionStun>().maxFallTime = 1.5f;
                }
            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(new Ray(this.transform.position,  -this.transform.forward));
    }

}
