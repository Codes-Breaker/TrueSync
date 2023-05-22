using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollActivator : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody body;
    public Animator animator;
    public List<Collider> ragdollCollider;
    public GrounderQuadruped groundIK;
    void Start()
    {

    }

    public void Ragdoll(bool isRagdoll)
    {
        groundIK.weight = isRagdoll ? 0 : 1;
        animator.enabled = !isRagdoll;
        body.detectCollisions = !isRagdoll;
        foreach(var col in ragdollCollider)
        {
            Physics.IgnoreCollision(body.GetComponent<Collider>(), col);
            col.GetComponent<Rigidbody>().isKinematic = !isRagdoll;
            col.GetComponent<Rigidbody>().detectCollisions = isRagdoll;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
